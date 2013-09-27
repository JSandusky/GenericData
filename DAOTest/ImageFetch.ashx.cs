using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAOTest {
    /// <summary>
    /// Summary description for ImageFetch
    /// </summary>
    public class ImageFetch : IHttpHandler {
        internal class DrawingRec {
            public DateTime time;
            public System.Drawing.Image img;
        }
        static Dictionary<Guid,DrawingRec> reqs;

        public static Guid addRequest(System.Drawing.Image image) {
            if (reqs == null)
                reqs = new Dictionary<Guid, DrawingRec>();
            foreach (Guid key in reqs.Keys) {
                DrawingRec rec = reqs[key];
                
                if (DateTime.Now.Subtract(rec.time).Hours > 1)
                    reqs.Remove(key);
            }
            Guid id = Guid.NewGuid();
            reqs[id] = new DrawingRec { time = DateTime.Now, img = image };
            return id;
        }

        public void ProcessRequest(HttpContext context) {
            if (reqs != null) {
                Guid id = Guid.Parse(context.Request["id"].ToString());
                DrawingRec rec = reqs[id];
                if (rec != null) {
                    using (System.IO.MemoryStream m = new System.IO.MemoryStream()) {
                        rec.img.Save(m, rec.img.RawFormat);
                        context.Response.BinaryWrite(m.ToArray());
                        context.Response.Flush();
                    }
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