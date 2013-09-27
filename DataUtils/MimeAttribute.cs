using System;
using System.Collections.Generic;
using System.Text;

namespace DataUtils {
    [AttributeUsage(AttributeTargets.Property)]
    public class MimeAttribute : Attribute {
        public string MimeType { get; set; }

    }
}
