using System;
using System.Collections.Generic;
using System.Text;

namespace DataUtils {
    [Serializable]
    public class DataObject {
        public int _dbId { get; internal set; }
        public virtual string getName() { return ""; }
    }
}
