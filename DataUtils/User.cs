using System;
using System.Collections.Generic;
using System.Text;

namespace DataUtils {
    [Serializable]
    public class UserAccount : DataObject {
        public string UserName { get; set; }
        public string UserPass { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime Created { get; set; }
        public DateTime BanDate { get; set; }
        public DateTime BanUntil { get; set; }
        public bool Admin { get; set; }
        public System.Drawing.Color TestColor {get;set;}
        public System.Drawing.Image TestImage { get; set; }

        [Mime(MimeType="application/pdf")]
        public byte[] TestFileData { get; set; }
        public ItemData SubItem { get; set; }
        public List<ItemData> OwnedItems { get; set; }

        public override string getName() {
            return UserName;
        }
    }

    [Serializable]
    public class ItemData : DataObject {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string ItemImage { get; set; }

        public override string getName() {
            return ItemName;
        }
    }
}
