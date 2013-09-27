using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using DataUtils;

namespace DAOTest {
    public partial class DataObjectPicker : System.Web.UI.UserControl {

        protected IList Items {
            get { return ViewState["Items"] as IList; }
            set { ViewState["Items"] = value; }
        }

        public int Selected {
            get {
                return ViewState["Selected"] != null ? (int)ViewState["Selected"] : 0;
            }
            set {
                ViewState["Selected"] = value;
            }
        }

        public string DefaultText {
            get {
                return ViewState["Text"] as String;
            }
            set {
                ViewState["Text"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (Items != null)
                bind();
        }

        public void setItems(IList list) {
            Items = list;
            if (ddlPicker != null)
                bind();
        }

        void bind() {
            ddlPicker.Items.Clear();
            ddlPicker.Items.Add(new ListItem("-- Select " + DefaultText + " --", "0"));
            for (int i = 0; i < Items.Count; ++i) {
                DataObject obj = Items[i] as DataObject;
                if (obj != null)
                    ddlPicker.Items.Add(new ListItem(obj.getName(), obj._dbId.ToString()));
                if (Selected > 0)
                    ddlPicker.SelectedValue = Selected.ToString();
            }
        }

        public DataObject getSelected() {
            if (ddlPicker.SelectedIndex > 0)
                return Items[ddlPicker.SelectedIndex - 1] as DataObject;
            return null;
        }

        protected void OnSelChg(object sender, EventArgs e) {
            Selected = ddlPicker.SelectedIndex;
        }
    }
}