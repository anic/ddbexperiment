using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscFragmentViewer : UserControl
    {
        public UscFragmentViewer()
        {
            InitializeComponent();
        }

        public void ShowSchema(TableSchema schema)
        {
            uscSchemaViewer.ShowTableSchema(schema);
            pnlLayout.ColumnStyles[1].Width = 0;
        }

        public void ShowFragment(Fragment f,GlobalDirectory gdd)
        {
            ShowFragment(f, true, true, true);
            pnlLayout.ColumnStyles[1].Width = 30;
            ShowFragmentView(f, gdd);
        }

        Fragment curFragment;
        public void ShowFragment(Fragment f, bool bShowAttrType, bool bShowCondition, bool bShowSite)
        {
            curFragment = f;

            this.uscSchemaViewer.ShowTableSchema(f.Schema, bShowAttrType);



            if (bShowSite)
                uscSchemaViewer.CurrentSite = f.Site;
            else
                uscSchemaViewer.CurrentSite = null;


            if (f.Condition != null && bShowCondition)
                uscSchemaViewer.Condition = f.Condition.ToString();
            else
                uscSchemaViewer.Condition = "";

        }

        public void ShowFragmentView(Fragment fragment, GlobalDirectory gdd)
        {
            uscFragmentShower.ShowFragment(fragment, gdd);
        }
    }
}
