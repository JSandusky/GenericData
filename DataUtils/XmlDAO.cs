using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Drawing;

namespace DataUtils {
    public class XmlDAO<T> : GenericDAO<T> where T : DataObject, new() {
        Type workingType_;
        string fileName_;
        PropertyInfo[] fields;
        int currentId_;

        public XmlDAO(string file) : base(new XmlTypeHandler()) {
            fileName_ = file;
            workingType_ = typeof(T);
            fields = workingType_.GetProperties();
        }

        public XmlDAO(string file, Type type) : base(new XmlTypeHandler()) {
            fileName_ = file;
            workingType_ = type;
            fields = workingType_.GetProperties();
        }

        public override GenericDAO<T> emulate(Type t) {
            return new XmlDAO<T>(fileName_, t);
        }

        public override GenericDAO<X> emulate<X>(Type t) {
            return new XmlDAO<X>(fileName_, t);
        }

        public override List<T> get(string query, int ct) {
            List<T> ret = new List<T>();

            XmlDocument xd = getDoc();

            XmlNodeList nl = xd.GetElementsByTagName(workingType_.Name);
            foreach (XmlNode nd in nl) {
                XmlElement elem = nd as XmlElement;
                if (elem != null)
                    ret.Add(mapEntry(elem));
            }

            return ret;
        }

        public override T getSingle(string query) {
            throw new NotImplementedException();
        }

        private T mapEntry(XmlElement root) {
            T ret = Activator.CreateInstance(workingType_) as T;
            foreach (XmlNode field in root.ChildNodes) {
                XmlElement fld = field as XmlElement;
                foreach (PropertyInfo pi in fields) {
                    if (pi.Name.Equals(field.Name)) {
                        string fldVal = fld.GetAttribute("value");

                        if (pi.PropertyType == typeof(int)) {
                            pi.SetValue(ret, int.Parse(fldVal), null);
                        } else if (pi.PropertyType == typeof(float)) {
                            pi.SetValue(ret, float.Parse(fldVal), null);
                        } else if (pi.PropertyType == typeof(double)) {
                            pi.SetValue(ret, double.Parse(fldVal), null);
                        } else if (pi.PropertyType == typeof(bool)) {
                            pi.SetValue(ret, bool.Parse(fldVal), null);
                        } else if (pi.PropertyType == typeof(string)) {
                            pi.SetValue(ret, fldVal, null);
                        } else if (pi.PropertyType == typeof(DateTime)) {
                            pi.SetValue(ret, DateTime.Parse(fldVal), null);
                        } else if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                            if (int.Parse(fldVal) > 0) {
                                DataObject subThing = Activator.CreateInstance(pi.PropertyType) as DataObject;
                                if (subThing != null) {
                                    subThing._dbId = int.Parse(fldVal);
                                    pi.SetValue(ret, subThing, null);
                                }
                            }
                        } else if (pi.PropertyType == typeof(Color)) {
                            if (!string.IsNullOrEmpty(fldVal)) {
                                Color col = (Color)System.Drawing.ColorTranslator.FromHtml(fldVal);
                                pi.SetValue(ret, col, null);
                            }
                        } else if (pi.PropertyType == typeof(Image)) {
                            string data = fld.InnerText;
                            if (!string.IsNullOrEmpty(data)) {
                                Image img = Base64ToImage(data);
                                if (img != null) {
                                    pi.SetValue(ret, img, null);
                                }
                            }
                        } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                            var listType = typeof(List<>);
                            Type liType = pi.PropertyType.GetGenericArguments()[0];
                            var constructedListType = listType.MakeGenericType(pi.PropertyType.GetGenericArguments()[0]);
                            IList li = Activator.CreateInstance(constructedListType) as IList;
                            if (li != null) {
                                string val = fldVal;
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
                    
                        }
                    }
                }
            }
            return ret;
        }

        public override void saveOrUpdate(T obj) {
            XmlDocument xd = getDoc();
            if (obj._dbId > 0) {
                XmlNodeList nl = xd.GetElementsByTagName(workingType_.Name);
                foreach (XmlNode node in nl) {
                    foreach (XmlNode field in node.ChildNodes) {
                        if (!field.Name.Contains("_")) {
                            XmlElement fld = field as XmlElement;
                            foreach (PropertyInfo pi in fields) {
                                if (pi.Name.Equals(field.Name)) {
                                    if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                                        DataObject agg = pi.GetValue(obj, null) as DataObject;
                                        if (agg != null)
                                            fld.SetAttribute("value", agg._dbId.ToString());
                                        else
                                            fld.SetAttribute("value", "0");
                                    } else if (pi.PropertyType == typeof(Color)) {
                                        Color col = (Color)pi.GetValue(obj, null);
                                        if (col != null) {
                                            fld.SetAttribute("value", System.Drawing.ColorTranslator.ToHtml(col));
                                        } else {
                                            fld.SetAttribute("value", "");
                                        }
                                    } else if (pi.PropertyType == typeof(Image)) {
                                        Image img = (Image)pi.GetValue(obj, null);
                                        if (img != null) {
                                            fld.SetAttribute("type", img.RawFormat.Guid.ToString());
                                            fld.InnerText = ImageToBase64(img, img.RawFormat);
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
                                            fld.SetAttribute("value", sb.ToString());
                                        } else {
                                            fld.SetAttribute("value", "");
                                        }
                                    } else {
                                        fld.SetAttribute("value", pi.GetValue(obj, null).ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            } else { //add it
                obj._dbId = currentId_+1;
                ++currentId_;
                XmlElement cl = xd.CreateElement(workingType_.Name);
                foreach (PropertyInfo pi in fields) {
                    if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                        XmlElement fld = xd.CreateElement(pi.Name);
                        DataObject agg = pi.GetValue(obj, null) as DataObject;
                        if (agg != null)
                            fld.SetAttribute("value", agg._dbId.ToString());
                        else
                            fld.SetAttribute("value", "0");
                        cl.AppendChild(fld);
                    } else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {
                        XmlElement fld = xd.CreateElement(pi.Name);
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
                            fld.SetAttribute("value", sb.ToString());
                        } else {
                            fld.SetAttribute("value", "");
                        }
                        cl.AppendChild(fld);
                    } else if (pi.PropertyType == typeof(Color)) {
                        XmlElement fld = xd.CreateElement(pi.Name);
                        Color col = (Color)pi.GetValue(obj, null);
                        if (col != null) {
                            fld.SetAttribute("value", System.Drawing.ColorTranslator.ToHtml(col));
                        } else {
                            fld.SetAttribute("value", "");
                        }
                        cl.AppendChild(fld);
                    } else if (pi.PropertyType == typeof(Image)) {
                        XmlElement fld = xd.CreateElement(pi.Name);
                        Image img = (Image)pi.GetValue(obj, null);
                        if (img != null) {
                            fld.SetAttribute("type", img.RawFormat.Guid.ToString());
                            fld.InnerText = ImageToBase64(img, img.RawFormat);
                        }
                        cl.AppendChild(fld);
                    } else {
                        XmlElement fld = xd.CreateElement(pi.Name);
                        fld.SetAttribute("value", pi.GetValue(obj, null).ToString());
                        cl.AppendChild(fld);
                    }
                }
                xd.GetElementsByTagName("data").Item(0).AppendChild(cl);
            }
            ((XmlElement)xd.GetElementsByTagName("data").Item(0)).SetAttribute("nextid", currentId_.ToString());
            xd.Save(fileName_);
        }

        public override void delete(T obj) {

            XmlDocument xd = getDoc();

            XmlNodeList nl = xd.GetElementsByTagName(workingType_.Name);
            bool found = false;
            if (nl.Count > 0 && obj._dbId > 0) {
                foreach (XmlNode nd in nl) {
                    if (found)
                        break;
                    foreach (XmlNode fld in nd.ChildNodes) {
                        XmlElement elem = fld as XmlElement;
                        if (fld.Name.Contains("_") && elem.GetAttribute("value").Equals(obj._dbId.ToString())) {
                            found = true;
                            xd.RemoveChild(nd);
                            break;
                        }
                    }
                }
            }
            ((XmlElement)xd.GetElementsByTagName("data").Item(0)).SetAttribute("nextid", currentId_.ToString());
            xd.Save(fileName_);
        }

        //reads or creates a new XML doc
        XmlDocument getDoc() {
            currentId_ = 0;
            XmlDocument xd = new XmlDocument();
            try {
                using (FileStream fs = new FileStream(fileName_, FileMode.Open)) {
                    xd.Load(fs);
                    currentId_ = int.Parse(((XmlElement)xd.GetElementsByTagName("data").Item(0)).GetAttribute("nextid"));
                }
            } catch (Exception) {
                XmlNode declaration = xd.CreateNode(XmlNodeType.XmlDeclaration, null, null);
                xd.AppendChild(declaration);
                xd.AppendChild(xd.CreateElement("data"));
            }
            return xd;
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
