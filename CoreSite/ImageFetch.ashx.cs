using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using DataUtils;

namespace CoreSite {
    /// <summary>
    /// Summary description for ImageFetch
    /// </summary>
    public class ImageFetch : IHttpHandler {

        public void ProcessRequest(HttpContext context) {
            int id = int.Parse(context.Request["id"].ToString());
            SqlDAO<BlogImage> bi = new SqlDAO<BlogImage>(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
            BlogImage img = bi.getSingle("WHERE _dbId = " + id);
            if (img != null && img.ImageData != null) {
                using (System.IO.MemoryStream m = new System.IO.MemoryStream()) {
                    img.ImageData.Save(m, img.ImageData.RawFormat);
                    context.Response.BinaryWrite(m.ToArray());
                    context.Response.Flush();
                }
            }
        }

        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}