using System;
using System.Collections.Generic;
using System.Web;
using DataUtils;
using System.Drawing;

namespace CoreSite {
    [Serializable]
    public class Blog : DataObject {
        public string PostTitle { get; set; }
        public string PostBy { get; set; }
        public DateTime PostDate { get; set; }
        public bool IsVisible { get; set; }
        public bool IsNews { get; set; }
        public List<BlogSection> Sections { get; set; }
        public List<BlogAtachment> Attachments { get; set; }
        public List<BlogTag> Tags { get; set; }
    }

    [Serializable]
    public class BlogSection : DataObject {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public BlogImage LeftImage { get; set; }
        public BlogImage RightImage { get; set; }
        public BlogImage AboveImage { get; set; }
        public BlogImage BelowImage { get; set; }
    }

    [Serializable]
    public class BlogImage : DataObject {
        public string ImageName { get; set; }
        public string ImageDesc { get; set; }
        public Image ImageData { get; set; }
    }

    [Serializable]
    public class BlogTag : DataObject {
        public string TagName { get; set; }
    }

    [Serializable]
    public class BlogAtachment : DataObject {
        public string AttachmentName { get; set; }
        public string AttachmentDescription { get; set; }
        
        [Mime(MimeType="application/zip")]
        public byte[] FileData { get; set; }
    }
}