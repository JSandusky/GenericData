using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using DataUtils;

namespace CoreSite {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            SqlDAO<Blog> blDao = new SqlDAO<Blog>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            List<Blog> bls = blDao.get("ORDER BY PostDate DESC",0);
            Loader.load<Blog>(bls, blDao.emulate<DataObject>(typeof(Blog)));
            rptNews.DataSource = bls;
            rptNews.DataBind();
        }

        protected void onBindNews(object sender, RepeaterItemEventArgs e) {
            Repeater rpt = e.Item.FindControl("rptSections") as Repeater;
            rpt.DataSource = ((Blog)e.Item.DataItem).Sections;
            rpt.DataBind();
        }

        protected void onBindSection(object sender, RepeaterItemEventArgs e) {
            Image img = e.Item.FindControl("sectionImage") as Image;
            Label lbl = e.Item.FindControl("sectionTxt") as Label;
            lbl.Text = ((BlogSection)e.Item.DataItem).Content;
            if (((BlogSection)e.Item.DataItem).BelowImage != null && ((BlogSection)e.Item.DataItem).BelowImage._dbId > 0 && ((BlogSection)e.Item.DataItem).BelowImage.ImageData != null) {
                img.Visible = true;
                img.ImageUrl = string.Format("~/ImageFetch.ashx?id={0}", ((BlogSection)e.Item.DataItem).BelowImage._dbId);
            }
        }
    }
}