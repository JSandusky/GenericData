using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataUtils;
using System.IO;
using System.Configuration;

namespace CoreSite.Admin {
    public partial class BlogEditor : System.Web.UI.Page {

        Blog blog { 
            get { 
                return ViewState["Blog"] as Blog; 
            } 
            set { 
                ViewState["Blog"] = value; 
            } 
        }
        int id { 
            get { 
                int ret = 0;
                int.TryParse(Request["id"], out ret);
                return ret;
            } 
        }

        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                if (id > 0) {
                    SqlDAO<Blog> bldao = new SqlDAO<Blog>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                    Blog bl = bldao.getSingle("WHERE _dbId = " + id);
                    if (bl != null) {
                        Loader.load(bl, bldao.emulate<DataObject>(typeof(Blog)));
                        blog = bl;
                    }
                } else {
                    Blog t = new Blog();
                    t.Sections = new List<BlogSection>();
                    t.Tags = new List<BlogTag>();
                    t.Attachments = new List<BlogAtachment>();
                    this.blog = t;
                }
                bind();
            }
        }

        public void onAddSection(object sender, EventArgs e) {
            save();
            if (blog.Sections == null)
                blog.Sections = new List<BlogSection>();
            blog.Sections.Add(new BlogSection());
            bind();
        }

        public void contentDataBind(object sender, RepeaterItemEventArgs e) {
            if (e.Item.ItemType == ListItemType.Item) {
                BlogSection sec = ((BlogSection)e.Item.DataItem);
                if (sec != null)
                    Loader.load(sec, new SqlDAO<DataObject>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString, typeof(BlogSection)));
                TextBox tb = e.Item.FindControl("txtContent") as TextBox;
                tb.Text = ((BlogSection)e.Item.DataItem).Content;
                Image img = e.Item.FindControl("img") as Image;
                if (((BlogSection)e.Item.DataItem).BelowImage != null && ((BlogSection)e.Item.DataItem).BelowImage._dbId > 0 && ((BlogSection)e.Item.DataItem).BelowImage.ImageData != null) {
                    img.Visible = true;
                    img.ImageUrl = string.Format("~/ImageFetch.ashx?id={0}", ((BlogSection)e.Item.DataItem).BelowImage._dbId);
                } else {
                    img.Visible = false;
                }
            }
        }

        protected void onSave(object sender, EventArgs e) {
            save();
            SqlDAO<Blog> sdao = new SqlDAO<Blog>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            Loader.save(blog,sdao.emulate<DataObject>(typeof(Blog)));
            sdao.saveOrUpdate(blog);
        }

        void save() {
            blog.PostTitle = txtTitle.Text;
            blog.IsNews = chkNews.Checked;
            blog.IsVisible = chkVis.Checked;
            for (int i = 0; i < rptSections.Items.Count; ++i) {
                TextBox tb = rptSections.Items[i].FindControl("txtContent") as TextBox;
                blog.Sections[i].Content = tb.Text;

                FileUpload fld = rptSections.Items[i].FindControl("flUpload") as FileUpload;
                if (fld.HasFile) {
                    try {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(fld.FileBytes));
                        BlogImage image = new BlogImage();
                        image.ImageData = img;
                        image.ImageName = fld.FileName;
                        image.ImageDesc = "";
                        blog.Sections[i].BelowImage = image;
                    } catch (Exception) {
                    }
                }
            }
        }

        void commit() {

        }

        void bind() {
            txtTitle.Text = blog.PostTitle;
            chkNews.Checked = blog.IsNews;
            chkVis.Checked = blog.IsVisible;
            if (blog.Sections != null) {
                rptSections.DataSource = blog.Sections;
                rptSections.DataBind();
            }
        }
    }
}