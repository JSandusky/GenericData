using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataUtils;
using System.Reflection;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace DAOTest {
    public partial class ReflectiveForm : System.Web.UI.UserControl {
        private const string CAPS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public DataObject source {
            get {
                return ViewState["source"] as DataObject;
            }
            set {
                ViewState["source"] = value;
            }
        }

        public bool DisplayOnly {
            get {
                return ViewState["readonly"] != null ? (bool)ViewState["readonly"] : false;
            }
            set {
                ViewState["readonly"] = value;
            }
        }

        public List<PropertyInfo> Properties {
            get {
                return ViewState["Properties"] as List<PropertyInfo>;
            }
            set {
                ViewState["Properties"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                source = new DataUtils.UserAccount();
                PropertyInfo[] infos = source.GetType().GetProperties();
                Properties = new List<PropertyInfo>(infos);
                for (int i = 0; i < Properties.Count; ++i) {
                    if (Properties[i].Name.Contains("_")) {
                        Properties.RemoveAt(i);
                        --i;
                    }
                }
                contents.DataSource = Properties;
                contents.DataBind();
            } else {
                contents.DataSource = Properties;
                contents.DataBind();
            }
        }

        public void setSource(DataObject source) {
            this.source = source;
            lblTitle.Text = "Edit " + makeLabel(source.GetType().Name);
            if (!string.IsNullOrEmpty(source.getName()))
                lblTitle.Text += ": " + source.getName();
            PropertyInfo[] infos = source.GetType().GetProperties();
            Properties = new List<PropertyInfo>(infos);
            for (int i = 0; i < Properties.Count; ++i) {
                if (Properties[i].Name.Contains("_")) {
                    Properties.RemoveAt(i);
                    --i;
                }
            }
            contents.DataSource = Properties;
            contents.DataBind();
        }

        protected void onDataBind(object sender, RepeaterItemEventArgs e) {
            Panel pnl = e.Item.FindControl("contentPanel") as Panel;
            Panel ctrlPnl = e.Item.FindControl("controlPanel") as Panel;
            PropertyInfo pi = e.Item.DataItem as PropertyInfo;


            if (pi.PropertyType == typeof(int)) {
                pnl.Controls.Add(makeLabel(pi));

                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        lbl.Text = pi.GetValue(source, null).ToString();
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    TextBox tb = new TextBox();
                    try {
                        tb.Text = pi.GetValue(source, null).ToString();
                    } catch (NullReferenceException ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    tb.ID = pi.Name;
                    ctrlPnl.Controls.Add(tb);
                }
            } else if (pi.PropertyType == typeof(float)) {
                pnl.Controls.Add(makeLabel(pi));
                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        lbl.Text = pi.GetValue(source, null).ToString();
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    TextBox tb = new TextBox();
                    tb.ID = pi.Name;
                    try {
                        tb.Text = pi.GetValue(source, null).ToString();
                    } catch (NullReferenceException ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(tb);
                }
            } else if (pi.PropertyType == typeof(double)) {
                pnl.Controls.Add(makeLabel(pi));
                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        lbl.Text = pi.GetValue(source, null).ToString();
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    TextBox tb = new TextBox();
                    tb.ID = pi.Name;
                    try {
                        tb.Text = pi.GetValue(source, null).ToString();
                    } catch (NullReferenceException ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(tb);
                }
            } else if (pi.PropertyType == typeof(string)) {
                pnl.Controls.Add(makeLabel(pi));
                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        lbl.Text = pi.GetValue(source, null).ToString();
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    TextBox tb = new TextBox();
                    tb.ID = pi.Name;
                    tb.TextMode = TextBoxMode.MultiLine;
                    try {
                        tb.Text = pi.GetValue(source, null).ToString();
                    } catch (NullReferenceException ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(tb);
                }
            } else if (pi.PropertyType == typeof(bool)) {
                CheckBox cb = new CheckBox();
                cb.Text = pi.Name;
                cb.ID = pi.Name;
                cb.Enabled = !DisplayOnly;
                try {
                    cb.Checked = (bool)pi.GetValue(source, null);
                } catch (NullReferenceException ex) {
                    Log.getInst("UI").error(ex.ToString());
                }
                ctrlPnl.Controls.Add(cb);
            } else if (pi.PropertyType == typeof(DateTime)) {
                pnl.Controls.Add(makeLabel(pi));
                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        lbl.Text = pi.GetValue(source, null).ToString();
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    TextBox tb = new TextBox();
                    tb.ID = pi.Name;
                    try {
                        tb.Text = pi.GetValue(source, null).ToString();
                    } catch (NullReferenceException ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    AjaxControlToolkit.CalendarExtender calExt = new AjaxControlToolkit.CalendarExtender();
                    calExt.TargetControlID = tb.ID;
                    ctrlPnl.Controls.Add(tb);
                    ctrlPnl.Controls.Add(calExt);
                }
            } else if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                pnl.Controls.Add(makeLabel(pi));
                if (DisplayOnly) {
                    Label lbl = new Label();
                    try {
                        DataObject cur = pi.GetValue(source, null) as DataObject;
                        if (cur != null) {
                            lbl.Text = string.IsNullOrEmpty(cur.getName()) ? "&lt;unnamed&gt;" : cur.getName();
                        } else {
                            lbl.Text = "&lt;unspecified&gt;";
                        }
                    } catch (Exception ex) {
                        Log.getInst("UI").error(ex.ToString());
                    }
                    ctrlPnl.Controls.Add(lbl);
                } else {
                    DataObjectPicker ddl = Page.LoadControl("DataObjectPicker.ascx") as DataObjectPicker;
                    ddl.ID = pi.Name;
                    ddl.setItems((IList)new TypeListHandler(pi.PropertyType).getContents());
                    DataObject cur = pi.GetValue(source, null) as DataObject;
                    ddl.DefaultText = makeLabel(pi.PropertyType.Name);
                    if (cur != null)
                        ddl.Selected = cur._dbId;
                    ctrlPnl.Controls.Add(ddl);
                }
            //} else if (typeof(IList).IsAssignableFrom(pi.PropertyType)) {

            } else if (pi.PropertyType == typeof(System.Drawing.Image)) {
                pnl.Controls.Add(makeLabel(pi));

                try {
                    System.Drawing.Image img = pi.GetValue(source, null) as System.Drawing.Image;
                    if (img != null) {
                        Guid id = ImageFetch.addRequest(img);
                        Image image = new Image();
                        image.ImageUrl = string.Format("ImageFetch.ashx?id={0}",id);
                        ctrlPnl.Controls.Add(image);
                    }
                } catch (Exception ex) {
                    Log.getInst("UI").error(ex.ToString());
                }

                FileUpload fl = new FileUpload();
                fl.ID = pi.Name;
                ctrlPnl.Controls.Add(fl);
            } else if (pi.PropertyType == typeof(System.Drawing.Color)) {
                pnl.Controls.Add(makeLabel(pi));

                TextBox tb = new TextBox();
                tb.ID = pi.Name;

                System.Drawing.Color val = (System.Drawing.Color)pi.GetValue(source, null);
                if (val != null) {
                    tb.Text = System.Drawing.ColorTranslator.ToHtml(val);
                }

                AjaxControlToolkit.ColorPickerExtender ext = new AjaxControlToolkit.ColorPickerExtender();
                ext.TargetControlID = pi.Name;
                ext.SampleControlID = tb.ID;
                ext.SelectedColor = tb.Text.Replace("#","");
                ctrlPnl.Controls.Add(tb);
                ctrlPnl.Controls.Add(ext);


            } else if (pi.PropertyType == typeof(byte[])) {
                pnl.Controls.Add(makeLabel(pi));

                try {
                    
                    byte[] data = pi.GetValue(source, null) as byte[];
                    if (data != null) {
                        HyperLink lnk = new HyperLink();
                        lnk.Text = "Has Data";
                        var attrs = pi.GetCustomAttributes(typeof(MimeAttribute), true);
                        string mime = "";
                        if (attrs != null && attrs.Length > 0) {
                            for (int i = 0; i < attrs.Length; ++i) {
                                MimeAttribute attr = attrs[i] as MimeAttribute;
                                mime = attr.MimeType;
                            }
                        }
                        Guid id = FileFetch.addRequest(data,mime);
                        lnk.NavigateUrl = string.Format("FileFetch.ashx?id={0}",id);
                        ctrlPnl.Controls.Add(lnk);
                    }
                } catch (Exception ex) {
                    Log.getInst("UI").error(ex.ToString());
                }

                FileUpload fl = new FileUpload();
                fl.ID = pi.Name;
                ctrlPnl.Controls.Add(fl);
            }
        }

        bool SaveData() {
            bool anyFailed = false;
            for (int i = 0; i < contents.Items.Count; ++i) {
                RepeaterItem item = contents.Items[i];
                Panel pnl = item.FindControl("controlPanel") as Panel;

                for (int c = 0; c < pnl.Controls.Count; ++c) {
                    Control ctrl = pnl.Controls[c];
                    if (ctrl is TextBox) {
                        TextBox tb = ctrl as TextBox;
                        PropertyInfo pi = getProperty(tb.ID);
                        anyFailed |= setValue(source, pi, tb.Text);
                    } else if (ctrl is CheckBox) {
                        CheckBox cb = ctrl as CheckBox;
                        PropertyInfo pi = getProperty(cb.ID);
                        try {
                            pi.SetValue(source, cb.Checked, null);
                        } catch (Exception ex) {
                            Log.getInst("UI").error(ex.ToString());
                            anyFailed = true;
                        }
                    } else if (ctrl is DataObjectPicker) {
                        DataObjectPicker ddl = ctrl as DataObjectPicker;
                        DataObject sel = ddl.getSelected();
                        PropertyInfo pi = getProperty(ddl.ID);
                        if (sel != null) //{
                            pi.SetValue(source, sel, null);
                        /*} else
                            anyFailed = true;*/
                    } else if (ctrl is FileUpload) {
                        FileUpload fl = ctrl as FileUpload;
                        PropertyInfo pi = getProperty(fl.ID);
                        try {
                            if (fl.HasFile) {
                                if (pi.PropertyType == typeof(System.Drawing.Image)) {
                                    System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(fl.FileBytes));
                                    pi.SetValue(source, img, null);
                                } else {
                                    string mime = getMimeFromFile(fl.PostedFile);

                                    var attrs = pi.GetCustomAttributes(typeof(MimeAttribute), true);
                                    string allowedmime = "";
                                    if (attrs != null && attrs.Length > 0) {
                                        for (int s = 0; s < attrs.Length; ++s) {
                                            MimeAttribute attr = attrs[s] as MimeAttribute;
                                            allowedmime += attr.MimeType;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(allowedmime) || allowedmime.ToLower().Equals(mime.ToLower())) {
                                        pi.SetValue(source, fl.FileBytes, null);

                                    } else {
                                        anyFailed = true;
                                    }
                                }
                            }
                        } catch (Exception ex) {
                            Log.getInst("UI").error(ex.ToString());
                            anyFailed = true;
                        }
                    }
                }
            }
            return anyFailed;
        }

        Control makeLabel(PropertyInfo pi) {
            Label lbl = new Label();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pi.Name.Length; ++i) {
                if (CAPS.Contains("" + pi.Name[i]) && sb.Length > 0) {
                    sb.Append(" ");
                }
                sb.Append(pi.Name[i]);
            }
            lbl.Text = sb.ToString();
            lbl.Text += ": ";
            lbl.Style.Add("padding-left", "5px");
            return lbl;
        }

        string makeLabel(string str) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; ++i) {
                if (CAPS.Contains("" + str[i]) && sb.Length > 0) {
                    sb.Append(" ");
                }
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        PropertyInfo getProperty(string name) {
            return Properties.Find(x => x.Name == name);
        }

        bool setValue(object target, PropertyInfo pi, string text) {
            try {
                if (pi.PropertyType == typeof(int)) {
                    int val = int.Parse(text);
                    pi.SetValue(target, val, null);
                } else if (pi.PropertyType == typeof(float)) {
                    float val = float.Parse(text);
                    pi.SetValue(target, val, null);
                } else if (pi.PropertyType == typeof(double)) {
                    double val = double.Parse(text);
                    pi.SetValue(target, val, null);
                } else if (pi.PropertyType == typeof(bool)) {
                    if (text.ToLower().Equals("true") || text.ToLower().Equals("yes") || text.ToLower().Equals("1")) {
                        pi.SetValue(target, true, null); 
                    } else {
                        pi.SetValue(target, false, null);
                    }
                } else if (pi.PropertyType == typeof(DateTime)) {
                    DateTime dt = DateTime.Parse(text);
                    pi.SetValue(target, dt, null);
                } else if (pi.PropertyType == typeof(string)) {
                    pi.SetValue(target, text, null);
                } else if (typeof(DataObject).IsAssignableFrom(pi.PropertyType)) {
                    //??
                }else if (pi.PropertyType == typeof(System.Drawing.Color)) {
                    if (!text.Contains("#"))
                        text = "#" + text;
                    System.Drawing.Color? col = null;
                    if (text.Length > 1) {
                        col = System.Drawing.ColorTranslator.FromHtml(text);
                    }
                    if (col.HasValue) {
                        pi.SetValue(target, col.Value, null);
                    }
                }
            } catch (Exception ex) {
                Log.getInst("UI").error(ex.ToString());
                return true;
            }
            return false;
        }

        protected void OnSave(object sender, EventArgs e) {
            if (!SaveData()) {
                try {
                    SqlDAO<DataObject> dao = new SqlDAO<DataObject>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", source.GetType());
                    dao.saveOrUpdate(source);
                } catch (Exception ex) {
                    Log.getInst("UI").error(ex.ToString());
                }
                Log.getInst("UI").write(Server.MapPath("DebugUI.html"), 4);
            } else {
            }
        }

        protected void OnCancel(object sender, EventArgs e) {

        }

        protected void OnDelete(object sender, EventArgs e) {
            try {
                SqlDAO<DataObject> dao = new SqlDAO<DataObject>("Data Source=REVLOCAL-54\\SQLEXPRESS;Initial Catalog=FileStoreTest;Integrated Security=True;", source.GetType());
                dao.delete(source);
            } catch (Exception ex) {
                Log.getInst("UI").error(ex.ToString());
            }

            Log.getInst("UI").write(Server.MapPath("DebugUI.html"), 4);
        }

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags, out IntPtr ppwzMimeOut, int dwReserved);

        public static string getMimeFromFile(HttpPostedFile file)
        {
            IntPtr mimeout;

            int MaxContent = (int)file.ContentLength;
            if (MaxContent > 4096) MaxContent = 4096;

            byte[] buf = new byte[MaxContent];
            file.InputStream.Read(buf, 0, MaxContent);
            int result = FindMimeFromData(IntPtr.Zero, file.FileName, buf, MaxContent, null, 0, out mimeout, 0);

            if (result != 0)
            {
                Marshal.FreeCoTaskMem(mimeout);
                return "";
            }

            string mime = Marshal.PtrToStringUni(mimeout);
            Marshal.FreeCoTaskMem(mimeout);

            return mime.ToLower();
        }
    }
}