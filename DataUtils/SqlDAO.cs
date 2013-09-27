using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Transactions;

namespace DataUtils {

    /* Two different constructors for different reasons, uses Activator.CreateInstance rather than new T()
     * The point is that you can go 
     *  SqlDao<User>(connString)
     * 
     *  SqlDao<DataObject>(connString, pi.PropertyType)
     *  
    */
    public class SqlDAO<T> : GenericDAO<T> where T : DataObject, new() {
        DataTable DataColumns;
        string connectionStr;
        string create_;
        string prequery_;
        string save_;
        string update_;
        string tableName_;
        bool tableVerified_;
        Type objectType_;
        PropertyInfo[] fields;

        //Simple obvious version
        public SqlDAO(string Connection) : base(new SqlTypeHandler()) {
            connectionStr = Connection;
            objectType_ = typeof(T);
            commonInit();
        }

        //I want to be have as T, but I'm really objType
        //ie. SqlDao<DataObject>(connString, typeof(User)) --which means that it's really working with users, but they're being returned as their DataObject base type
        public SqlDAO(string Connection, Type objType) : base(new SqlTypeHandler()) {
            connectionStr = Connection;
            objectType_ = objType;
            commonInit();
        }

        public override GenericDAO<T> emulate(Type t) {
            return new SqlDAO<T>(connectionStr, t);
        }

        public override GenericDAO<X> emulate<X>(Type t) {
            return new SqlDAO<X>(connectionStr, t);
        }

        public string getTableName() {
            return tableName_;
        }

        void commonInit() {
            PropertyInfo[] props = objectType_.GetProperties();
            create_ = buildCreate(objectType_, props);
            prequery_ = buildQuery(objectType_, props);
            fields = props;
        }

        public void exec(string query) {
            using (SqlConnection con = new SqlConnection(connectionStr)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable execDT(string query) {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionStr)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    using (SqlDataAdapter adapt = new SqlDataAdapter())
                        adapt.Fill(dt);
                }
            }
            return dt;
        }

        public object execScalar(string query) {
            using (SqlConnection con = new SqlConnection(connectionStr)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    return cmd.ExecuteScalar();
                }
            }
        }

        public override T getSingle(string query) {
            DataTable tbl = getDataTable(query,1);
            for (int i = 0; i < tbl.Rows.Count; ) {
                return mapRow(tbl.Rows[i]);
            }
            return null;
        }

        public DataTable getDataTable(string query, int ct) {
            verifyTable();
            DataTable dt = new DataTable();
            using (TransactionScope scope = new TransactionScope()) {
                using (SqlConnection con = new SqlConnection(connectionStr)) {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(string.Format(prequery_, ct > 0 ? "TOP " + ct + " *": "*") + query, con)) {
                        using (SqlDataAdapter adapt = new SqlDataAdapter(cmd)) {
                            adapt.Fill(dt);
                        }
                    }
                }
                scope.Complete();
            }
            return dt;
        }

        public override List<T> get(string query, int ct) {
            List<T> ret = new List<T>();
            DataTable dt = getDataTable(query,ct);
            foreach (DataRow dr in dt.Rows)
                ret.Add(mapRow(dr));
            return ret;
        }

        public override void saveOrUpdate(T obj) {
            verifyTable();

            if (string.IsNullOrEmpty(save_))
                buildSaveProc(objectType_, fields);

            T thing = getSingle("WHERE _dbId = " + obj._dbId);

            using (TransactionScope scope = new TransactionScope()) {
                using (SqlConnection con = new SqlConnection(connectionStr)) {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(thing != null ? update_ : save_, con)) {
                        for (int i = 0; i < fields.Length; ++i) {
                            PropertyInfo pi = fields[i];
                            if (pi.Name.Contains("_") && thing != null)
                                continue;
                            if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                                DataObject val = pi.GetValue(obj, null) as DataObject;
                                if (val != null)
                                    cmd.Parameters.AddWithValue("@" + pi.Name, val._dbId);
                                else
                                    cmd.Parameters.AddWithValue("@" + pi.Name, 0);
                            } else if (pi.PropertyType == typeof(byte[])) {
                                byte[] val = pi.GetValue(obj, null) as byte[];
                                if (val != null) {
                                    SqlParameter p = new SqlParameter("@" + pi.Name, SqlDbType.VarBinary);
                                    p.Value = val;
                                    cmd.Parameters.Add(p);
                                } else {
                                    SqlParameter p = new SqlParameter("@" + pi.Name, SqlDbType.VarBinary);
                                    p.Value = DBNull.Value;
                                    cmd.Parameters.Add(p);
                                }
                                
                            } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                                IList li = pi.GetValue(obj, null) as IList;
                                if (li != null) {
                                    StringBuilder sb = new StringBuilder();
                                    for (int sub = 0; sub < li.Count; ++sub) {
                                        DataObject nest = li[sub] as DataObject;
                                        if (nest != null) {
                                            if (sb.Length > 0)
                                                sb.Append(",");
                                            sb.Append(nest._dbId);
                                        }
                                    }
                                    cmd.Parameters.AddWithValue("@" + pi.Name, sb.ToString());
                                } else {
                                    cmd.Parameters.AddWithValue("@" + pi.Name, "");
                                }
                            } else if (pi.PropertyType == typeof(DateTime)) {
                                DateTime val = (DateTime)pi.GetValue(obj, null);
                                cmd.Parameters.AddWithValue("@" + pi.Name, val != DateTime.MinValue ? (object)val : (object)DBNull.Value);
                            } else if (pi.PropertyType == typeof(Image)) {
                                Image val = (Image)pi.GetValue(obj, null);
                                if (val != null) {
                                    using (System.IO.MemoryStream m = new System.IO.MemoryStream()) {
                                        val.Save(m, val.RawFormat);
                                        SqlParameter p = new SqlParameter("@" + pi.Name, SqlDbType.Image);
                                        p.Value = m.ToArray();
                                        cmd.Parameters.Add(p);
                                    }
                                } else {
                                    SqlParameter p = new SqlParameter("@" + pi.Name, SqlDbType.Image);
                                    p.Value = DBNull.Value;
                                    cmd.Parameters.Add(p);
                                }
                            } else if (pi.PropertyType == typeof(Color)) {
                                Color col = (Color)pi.GetValue(obj, null);
                                if (col != null) {
                                    cmd.Parameters.AddWithValue("@" + pi.Name, System.Drawing.ColorTranslator.ToHtml(col));
                                } else {
                                    cmd.Parameters.AddWithValue("@" + pi.Name, DBNull.Value);
                                }
                            } else {
                                object val = pi.GetValue(obj, null);
                                cmd.Parameters.AddWithValue("@" + pi.Name, val != null ? val : DBNull.Value);
                            }
                        }
                        if (thing != null)
                            cmd.Parameters.AddWithValue("@_dbId", obj._dbId);
                        object ret = cmd.ExecuteScalar();
                        if (ret != null && ret.GetType() == typeof(Decimal)) {
                            obj._dbId = Convert.ToInt32(ret);
                        }
                    }
                }
                scope.Complete();
            }
        }

        public override void delete(T obj) {
            using (SqlConnection con = new SqlConnection(connectionStr)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM " + tableName_ + " WHERE _dbId = @dbID", con)) {
                    cmd.Parameters.AddWithValue("@dbID", obj._dbId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void verifyTable() {
            if (tableVerified_)
                return;
            Log.getInst("Data").debug(string.Format("Verifying table: {0}", tableName_));
            bool found = false;
            using (SqlConnection con = new SqlConnection(connectionStr)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName_ + "'", con)) {
                    using (SqlDataReader rdr = cmd.ExecuteReader()) {
                        if (rdr.Read()) {
                            found = true;
                            tableVerified_ = true;
                        }
                    }
                }
                if (found) {
                    DataTable columns = new DataTable();
                    using (SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName_ + "'", con)) {
                        using (SqlDataAdapter adapt = new SqlDataAdapter(cmd)) {
                            adapt.Fill(columns);
                        }
                    }
                    for (int i = 0; i < columns.Rows.Count; ++i) {
                        DataRow dr = columns.Rows[i];
                        bool propFound = false;
                        foreach (PropertyInfo pi in fields) {
                            if (dr[0].ToString().Equals(pi.Name)) {
                                propFound = true;
                                break;
                            }
                        }
                        if (!propFound) {
                            Log.getInst("Data").debug(string.Format("Droping Column From {0}: {1}", tableName_, dr[0].ToString()));
                            using (SqlCommand cmd = new SqlCommand("ALTER TABLE " + tableName_ + " DROP COLUMN " + dr[0].ToString(), con)) {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    foreach (PropertyInfo pi in fields) {
                        bool colFound = false;
                        for (int i = 0; i < columns.Rows.Count; ++i) {
                            DataRow dr = columns.Rows[i];
                            if (dr[0].ToString().Equals(pi.Name)) {
                                colFound = true;
                                break;
                            }
                        }
                        if (!colFound) {
                            Log.getInst("Data").debug(string.Format("Adding column to {0}: {1}", tableName_, pi.Name));
                            string sql = "ALTER TABLE " + tableName_ + " ADD " + pi.Name;
                            sql += TypeHandler.getDataTypeName(pi.PropertyType);
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                                cmd.ExecuteNonQuery();
                        }
                    }

                } else if (!found) {
                    using (SqlCommand cmd = new SqlCommand(create_, con)) {
                        cmd.ExecuteNonQuery();
                        tableVerified_ = true;
                    }
                }

                DataColumns = new DataTable();
                using (SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName_ + "'", con)) {
                    using (SqlDataAdapter adapt = new SqlDataAdapter(cmd)) {
                        adapt.Fill(DataColumns);
                    }
                }
            }
        }


        private string buildQuery(Type type, PropertyInfo[] props) {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT {0} FROM ");
            sb.Append(tableName_);
            sb.Append(" ");
            return sb.ToString();
        }

        private void buildSaveProc(Type type, PropertyInfo[] props) {
            StringBuilder save = new StringBuilder();
            StringBuilder update = new StringBuilder();
            save.Append("INSERT INTO ");
            save.Append(tableName_);
            save.Append(" VALUES (");

            update.Append("UPDATE ");
            update.Append(tableName_);
            update.Append(" SET ");

            bool first = true;
            foreach (DataRow dr in DataColumns.Rows) {
                if (!dr[0].ToString().Contains("_")) {
                    if (!first) {
                        save.Append(", ");
                        update.Append(", ");
                    }
                    first = false;

                    save.Append("@" + dr[0].ToString());

                    update.Append(dr[0].ToString());
                    update.Append(" = ");
                    update.Append("@" + dr[0].ToString());
                }
            }
            /*for (int i = 0; i < props.Length; ++i) {
                PropertyInfo pi = props[i];
                
                if (!pi.Name.Contains("_")) {
                    if (i > 0) {
                        save.Append(", ");
                        update.Append(", ");
                    }

                    save.Append("@"+pi.Name);

                    update.Append(pi.Name);
                    update.Append(" = ");
                    update.Append("@" + pi.Name);
                }
            }*/

            save.Append("); SELECT @@IDENTITY;");
            
            save_ = save.ToString();

            update.Append(" WHERE _dbId = @_dbId");
            update_ = update.ToString();
        }

        private string buildCreate(Type type, PropertyInfo[] props) {
            StringBuilder sb = new StringBuilder();
            tableName_ = type.Name;
            sb.Append("CREATE TABLE ");
            sb.Append(tableName_);
            sb.Append(" (");

            for (int i = 0; i < props.Length; ++i) {
                if (i > 0)
                    sb.Append(", ");

                PropertyInfo pi = props[i];
                sb.Append(pi.Name);
                if (pi.PropertyType == typeof(int)) {
                    sb.Append(" int");
                    if (pi.Name.Contains("_")) {
                        sb.Append(" IDENTITY(1,1) NOT NULL");
                    }
                } else {
                    sb.Append(" " + TypeHandler.getDataTypeName(pi.PropertyType));
                }
            }
            sb.Append(") ");
            return sb.ToString();
        }

        private T mapRow(DataRow dr) {
            T ret = Activator.CreateInstance(objectType_) as T;
            for (int i = 0; i < fields.Length; ++i) {
                PropertyInfo pi = fields[i];
                pi.SetValue(ret,TypeHandler.getDataTypeValueIn(dr[pi.Name],pi.PropertyType),null);
            }
            return ret;
        }
    }
}
