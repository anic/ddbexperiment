using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Table;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscTableViewer : UserControl
    {
        public UscTableViewer()
        {
            InitializeComponent();
        }

        Table table;
        public Table Table
        {
            get { return table; }
            set { 
                table = value;
                ShowTable(table);
            }
      
        }

        

        private void ShowTable(Table table)
        {
            lvwTable.Columns.Clear();
            lvwTable.Items.Clear();

            if (table == null)
                return;

            foreach (Field f in table.Schema.Fields)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = f.AttributeName + ":" + f.AttributeType.ToString();
                header.Width = header.Text.Length * 10 + 10;
                lvwTable.Columns.Add(header);
            }

            foreach (Tuple tuple in table.Tuples)
            {
                if (tuple.Data.Count > 0)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = tuple.Data[0];
                    for (int i = 1; i < tuple.Data.Count; i++)
                    {
                        ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
                        sub.Text = tuple.Data[i];
                        item.SubItems.Add(sub);
                    }

                    lvwTable.Items.Add(item);
                }
            }

            TableUtility util = new TableUtility();
            util.PaintCrossColor(lvwTable, Color.White, Color.LightGray);
            util.FitSize(lvwTable, lvwTable.Size);

        }


    }
}
