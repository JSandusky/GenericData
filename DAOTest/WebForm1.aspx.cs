using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataUtils;
using FTL;

namespace DAOTest {
    public partial class WebForm1 : System.Web.UI.Page {
        

        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                listFrom.setHandler(new ListHandler<DataUtils.UserAccount>());
                //listFrom.setSelectedIndexHandler(new EventHandler(OnGridSelectionChanged));
            }

            RandomGen gen = new RandomGen();
            gen.Length = 256;
            gen.MaxDeltaUp = 3;
            gen.MaxDeltaDown = 7;
            gen.MaxStep = 12;
            gen.MinStep = 3;
            gen.MinHeight = 2;
            gen.StartHeight = 3;
            gen.MaxHeight = 24;
            txtCSV.Text = gen.getString();
            //listFrom.SelectionChanged += OnGridSelectionChanged;

            Log.getInst("Data").write(Server.MapPath("DebugData.html"), 4);
        }

        protected void OnGridSelectionChanged(object sender, EventArgs e) {
            GridViewSelectEventArgs eargs = e as GridViewSelectEventArgs;
            object item = new ListHandler<DataUtils.UserAccount>().getItem(eargs.NewSelectedIndex);
            formThing.setSource((DataObject)item);
        }

        protected void onNew(object sender, EventArgs e) {
            formThing.setSource(new DataUtils.ItemData());
        }
    }
}