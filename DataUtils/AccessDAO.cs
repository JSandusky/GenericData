using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Drawing;
using System.IO;

namespace DataUtils {

    public class AccessDAO<T> : GenericDAO<T> where T : DataObject, new() {
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
        public AccessDAO(string Connection) : base(new AccessTypeHandler()) {
            connectionStr = Connection;
            objectType_ = typeof(T);
            commonInit();
        }

        //I want to be have as T, but I'm really objType
        //ie. SqlDao<DataObject>(connString, typeof(User)) --which means that it's really working with users, but they're being returned as their DataObject base type
        public AccessDAO(string Connection, Type objType) : base(new AccessTypeHandler()) {
            connectionStr = Connection;
            objectType_ = objType;
            commonInit();
        }

        public override GenericDAO<T> emulate(Type t) {
            return new AccessDAO<T>(connectionStr, t);
        }

        public override GenericDAO<X> emulate<X>(Type t) {
            return new AccessDAO<X>(connectionStr, t);
        }

        void commonInit() {
            PropertyInfo[] props = objectType_.GetProperties();
            create_ = buildCreate(objectType_, props);
            buildSaveProc(objectType_, props);
            prequery_ = buildQuery(objectType_, props);
            fields = props;
        }

        public override T getSingle(string query) {
            DataTable tbl = getDataTable(query,1);
            for (int i = 0; i < tbl.Rows.Count; ++i) {
                return mapRow(tbl.Rows[i]);
            }
            return null;
        }

        public DataTable getDataTable(string query, int ct) {
            verifyTable();
            DataTable dt = new DataTable();
            using (OleDbConnection con = new OleDbConnection(connectionStr)) {
                con.Open();
                using (OleDbCommand cmd = new OleDbCommand(string.Format(prequery_, ct > 0 ? "TOP " + ct : "*") + query, con)) {
                    using (OleDbDataAdapter adapt = new OleDbDataAdapter(cmd)) {
                        adapt.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public override List<T> get(string query, int ct) {
            List<T> ret = new List<T>();
            DataTable dt = getDataTable(query, ct);
            foreach (DataRow dr in dt.Rows)
                ret.Add(mapRow(dr));
            return ret;
        }

        public override void saveOrUpdate(T obj) {
            T thing = getSingle("WHERE _dbId = " + obj._dbId);

            using (OleDbConnection con = new OleDbConnection(connectionStr)) {
                con.Open();
                using (OleDbCommand cmd = new OleDbCommand(thing != null ? update_ : save_, con)) {
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
                        } else if (pi.PropertyType == typeof(Color)) {
                            Color col = (Color)pi.GetValue(obj, null);
                            if (col != null) {
                                cmd.Parameters.AddWithValue("@" + pi.Name, System.Drawing.ColorTranslator.ToHtml(col));
                            } else {
                                cmd.Parameters.AddWithValue("@" + pi.Name, DBNull.Value);
                            }
                        } else if (pi.PropertyType == typeof(Image)) {
                            Image img = (Image)pi.GetValue(obj, null);
                            if (img != null) {
                                cmd.Parameters.AddWithValue("@" + pi.Name, ImageToBase64(img,img.RawFormat));
                            } else {
                                cmd.Parameters.AddWithValue("@" + pi.Name, DBNull.Value);
                            }
                        } else if (pi.PropertyType == typeof(DateTime)) {
                            DateTime val = (DateTime)pi.GetValue(obj, null);
                            cmd.Parameters.AddWithValue("@" + pi.Name, val != DateTime.MinValue ? (object)val : (object)DBNull.Value);
                        } else {
                            cmd.Parameters.AddWithValue("@" + pi.Name, pi.GetValue(obj, null));
                        }
                    }
                    if (thing != null)
                        cmd.Parameters.AddWithValue("@_dbId", obj._dbId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override void delete(T obj) {
            using (OleDbConnection con = new OleDbConnection(connectionStr)) {
                con.Open();
                using (OleDbCommand cmd = new OleDbCommand("DELETE FROM " + tableName_ + " WHERE _dbId = @dbID", con)) {
                    cmd.Parameters.AddWithValue("@dbID", obj._dbId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        void verifyTable() {
            if (tableVerified_)
                return;
            bool found = false;
            using (OleDbConnection con = new OleDbConnection(connectionStr)) {
                con.Open();
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + tableName_ + "'", con)) {
                    using (OleDbDataReader rdr = cmd.ExecuteReader()) {
                        if (rdr.Read()) {
                            found = true;
                            tableVerified_ = true;
                            return;
                        }
                    }
                }

                if (found) {
                    DataTable columns = new DataTable();
                    using (OleDbCommand cmd = new OleDbCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName_ + "'", con)) {
                        using (OleDbDataAdapter adapt = new OleDbDataAdapter(cmd)) {
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
                            using (OleDbCommand cmd = new OleDbCommand("ALTER TABLE " + tableName_ + " DROP COLUMN " + dr[0].ToString(), con))
                                cmd.ExecuteNonQuery();
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
                            string sql = "ALTER TABLE " + tableName_ + " ADD " + pi.Name;
                            sql += TypeHandler.getDataTypeName(pi.PropertyType);
                            using (OleDbCommand cmd = new OleDbCommand(sql, con))
                                cmd.ExecuteNonQuery();
                        }
                    }
                } else if (!found) {
                    using (OleDbCommand cmd = new OleDbCommand(create_, con)) {
                        cmd.ExecuteNonQuery();
                        tableVerified_ = true;
                    }
                }
            }
        }

        private string buildQuery(Type type, PropertyInfo[] props) {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM ");
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

            for (int i = 0; i < props.Length; ++i) {
                PropertyInfo pi = props[i];

                if (!pi.Name.Contains("_")) {
                    if (i > 0) {
                        save.Append(", ");
                        update.Append(", ");
                    }

                    save.Append("@" + pi.Name);

                    update.Append(pi.Name);
                    update.Append(" = ");
                    update.Append("@" + pi.Name);
                }
            }

            save.Append(")");
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
                if (pi.Name.Contains("_")) {
                    sb.Append(" AUTOINCREMENT(1,1)");
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
                if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                    DataObject obj = Activator.CreateInstance(pi.PropertyType) as DataObject;
                    if (obj != null)
                        obj._dbId = (int)dr[pi.Name];
                    pi.SetValue(ret, obj, null);

                    AccessDAO<DataObject> sdao = new AccessDAO<DataObject>("", pi.PropertyType);
                } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                    var listType = typeof(List<>);
                    Type liType = pi.PropertyType.GetGenericArguments()[0];
                    var constructedListType = listType.MakeGenericType(pi.PropertyType.GetGenericArguments()[0]);
                    IList li = Activator.CreateInstance(constructedListType) as IList;
                    if (li != null) {
                        string val = dr[pi.Name].ToString();
                        string[] parts = val.Split(',');
                        foreach (string s in parts) {
                            DataObject ch = Activator.CreateInstance(liType) as DataObject;
                            if (ch != null) {
                                ch._dbId = int.Parse(s);
                                li.Add(ch);
                            }
                        }
                    }
                    pi.SetValue(ret, li, null);
                } else if (pi.PropertyType == typeof(Color)) {
                    string val = dr[pi.Name].ToString();
                    if (!string.IsNullOrEmpty(val)) {
                        Color col = (Color)ColorTranslator.FromHtml(val);
                        pi.SetValue(ret, col, null);
                    }
                } else if (pi.PropertyType == typeof(Image)) {
                    string data = dr[pi.Name].ToString();
                    if (!string.IsNullOrEmpty(data)) {
                        Image img = Base64ToImage(data);
                        pi.SetValue(ret, img, null);
                    }
                } else {
                    if (dr[i] != DBNull.Value) {
                        object val = dr[pi.Name];
                        pi.SetValue(ret, Convert.ChangeType(val, pi.PropertyType), null);
                    }
                }
            }
            return ret;
        }

        public string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format) {
            using (MemoryStream ms = new MemoryStream()) {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public Image Base64ToImage(string base64String) {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }
}
