using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAOTest {
    /// <summary>
    /// Summary description for FileFetch
    /// </summary>
    public class FileFetch : IHttpHandler {
        internal class FileRec {
            public DateTime time;
            public byte[] data;
            public string Mime;
        }
        static Dictionary<Guid, FileRec> reqs;

        public static Guid addRequest(byte[] datum, string mime) {
            if (reqs == null)
                reqs = new Dictionary<Guid, FileRec>();
            foreach (Guid key in reqs.Keys) {
                FileRec rec = reqs[key];

                if (DateTime.Now.Subtract(rec.time).Hours > 1)
                    reqs.Remove(key);
            }
            Guid id = Guid.NewGuid();
            reqs[id] = new FileRec { time = DateTime.Now, data = datum, Mime = mime };
            return id;
        }

        public void ProcessRequest(HttpContext context) {
            if (reqs != null) {
                Guid id = Guid.Parse(context.Request["id"].ToString());
                FileRec rec = reqs[id];
                if (rec != null) {
                    context.Response.ContentType = rec.Mime;
                    context.Response.BinaryWrite(rec.data);
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