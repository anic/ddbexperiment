using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscTableSchemaViewer : UserControl
    {
        public UscTableSchemaViewer()
        {
            InitializeComponent();
        }

        public void ShowTableSchema(TableSchema table)
        {
            lvwTable.Columns.Clear();
            lvwTable.Items.Clear();
            lblCondition.Text = "";

            lblTableName.Text = table.TableName;

            foreach(Field f in table.Fields)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = f.AttributeName;
                header.Width = f.AttributeName.Length * 10 + 10;
                lvwTable.Columns.Add(header);
            }

            if (table.Fields.Count>0)
            {
                ListViewItem item = new ListViewItem();
                item.Text = table.Fields[0].AttributeType.ToString();
                for (int i = 1; i < table.Fields.Count; i++)
                {
                    ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
                    sub.Text = table.Fields[i].AttributeType.ToString();
                    item.SubItems.Add(sub);
                }
                lvwTable.Items.Add(item);
            }
            
        }

        public void ShowFragment(Fragment f)
        {
            ShowTableSchema(f.Schema);

            lblTableName.Text = f.Name;
            if (f.Condition != null)
                lblCondition.Text = f.Condition.ToString();
        }
    }
}
