using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataUtils;

namespace DAOTest {
    public partial class ReflectiveList : System.Web.UI.UserControl  {

        public delegate void OnSelectDelegate(object sender, EventArgs e);

        public event OnSelectDelegate SelectionChanged;

        protected void Page_Load(object sender, EventArgs e) {
            
        }

        public void setHandler(ListHandlerBase li) {
            gvContents.DataSource = li.getContents();
            gvContents.DataBind();
        }

        public object getSelected(ListHandlerBase li, int i) {
            return li.getItem(i);
        }

        /*public void setSelectedIndexHandler(EventHandler e) {
            OnSelect += e;
        }*/

        protected void OnIndex(object sender, EventArgs e) {
            if (SelectionChanged != null)
                SelectionChanged(sender, new GridViewSelectEventArgs(gvContents.SelectedIndex));
        }
    }
}