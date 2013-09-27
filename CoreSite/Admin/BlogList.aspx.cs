using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataUtils;
using System.Configuration;

namespace CoreSite.Admin {
    public partial class BlogList : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            SqlDAO<Blog> sdao = new SqlDAO<Blog>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            List<Blog> results = sdao.get("", 0);
            Loader.load<Blog>(results, sdao.emulate<DataObject>(typeof(Blog)));
            rptExisting.DataSource = results;
            rptExisting.DataBind();
        }

        public void onNewPost(object sender, EventArgs e) {
            Response.Redirect("Admin/BlogEditor.aspx");
        }

        public void onRptBind(object sender, RepeaterItemEventArgs e) {
            Repeater subRpt = e.Item.FindControl("rptTags") as Repeater;
            HyperLink lnk = e.Item.FindControl("lnkTitle") as HyperLink;
            lnk.NavigateUrl = string.Format("~/Admin/BlogEditor.aspx?id={0}", ((Blog)e.Item.DataItem)._dbId);
        }
    }
}