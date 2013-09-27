using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.IO;

namespace DataUtils {
    public abstract class TypeHandler {
        public abstract string getDataTypeName(Type t);
        public abstract object getDataTypeValueOut(object thing, Type t);
        public abstract object getDataTypeValueIn(object thing, Type t);
    }

    public abstract class DBTypeHandler : TypeHandler {
        public override object getDataTypeValueOut(object thing, Type t) {
            return null;
        }

        public override object getDataTypeValueIn(object thing, Type t) {
            if (thing == DBNull.Value)
                return null;
            if (typeof(DataObject).IsAssignableFrom(t)) {
                DataObject obj = Activator.CreateInstance(t) as DataObject;
                if (obj != null)
                    obj._dbId = (int)thing;
                return obj;
            } else if (t == typeof(byte[])) {
                try {
                    return thing;
                } catch (Exception) {
                    return null;
                }
            } else if (typeof(IList).IsAssignableFrom(t)) {
                var listType = typeof(List<>);
                Type liType = t.GetGenericArguments()[0];
                var constructedListType = listType.MakeGenericType(t.GetGenericArguments()[0]);
                IList li = Activator.CreateInstance(constructedListType) as IList;
                if (li != null) {
                    string val = thing.ToString();
                    string[] parts = val.Split(',');
                    foreach (string s in parts) {
                        if (s == string.Empty)
                            continue;
                        DataObject ch = Activator.CreateInstance(liType) as DataObject;
                        if (ch != null) {
                            ch._dbId = int.Parse(s);
                            li.Add(ch);
                        }
                    }
                }
                return li;
            
            } else if (t == typeof(Image)) {
                try {
                    byte[] bytes = (byte[])thing;
                    Image img = Image.FromStream(new MemoryStream(bytes));
                    return img;
                } catch (Exception) {
                    return null;
                }
            } else if (t == typeof(Color)) {
                try {
                    Color col = (Color)System.Drawing.ColorTranslator.FromHtml((string)thing);
                    return col;
                } catch (Exception e) {
                    return null;
                }
            } else {
                if (thing != DBNull.Value)
                    return Convert.ChangeType(thing, t);
            }
            return null;
        }
    }

    internal class AccessTypeHandler : DBTypeHandler {
        public override string getDataTypeName(Type t) {
            if (t == typeof(int)) {
                return " int";
            } else if (t == typeof(string)) {
                return " varchar[2048]";
            } else if (t == typeof(float)) {
                return " float";
            } else if (t == typeof(double)) {
                return " double";
            } else if (t == typeof(bool)) {
                return " bit";
            } else if (t == typeof(DateTime)) {
                return " datetime";
            } else if (typeof(DataObject).IsAssignableFrom(t)) {
                return " Integer";
            } else if (t == typeof(byte[])) {
                return " varbinary[max]";
            } else if (typeof(IList).IsAssignableFrom(t)) {
                return " varchar[2048]";
            } else if (t == typeof(Image)) {
                return " varchar[max]";
            } else if (t == typeof(Color)) {
                return " varchar[10]";
            }
            return "";
        }
    }

    internal class SqlTypeHandler : DBTypeHandler {
        public override string getDataTypeName(Type t) {
            if (t == typeof(int)) {
                return " int";
            } else if (t == typeof(string)) {
                return " nvarchar(2048)";
            } else if (t == typeof(float)) {
                return " float";
            } else if (t == typeof(double)) {
                return " double";
            } else if (t == typeof(bool)) {
                return " bit";
            } else if (t == typeof(DateTime)) {
                return " datetime";
            } else if (typeof(DataObject).IsAssignableFrom(t)) {
                return " int";
            } else if (t == typeof(byte[])) {
                return " varbinary(max)";
            } else if (typeof(IList).IsAssignableFrom(t)) {
                return " nvarchar(2048)";
            } else if (t == typeof(Image)) {
                return " image";
            } else if (t == typeof(Color)) {
                return " varchar(10)";
            }
            return "";
        }        
    }

    internal class XmlTypeHandler : TypeHandler {
        public override string getDataTypeName(Type t) {
            return t.Name;
        }
        public override object getDataTypeValueOut(object thing, Type t) {
            return null;
        }
        public override object getDataTypeValueIn(object thing, Type t) {
            return null;
        }
    }
}
