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
    public partial class UscSchemaViewer : UserControl
    {
        public UscSchemaViewer()
        {
            InitializeComponent();
        }

        Site site;
        public Site CurrentSite
        {
            get{return site;}
            set{
                
                if (value == null)
                {
                    lblSite.Text = "";
                    lblSite.Visible = false;
                }
                else
                {
                    lblSite.Text = value.ToString();
                    lblSite.Visible = true;
                }
                site = value;
            }
        }

        public void ShowTableSchema(TableSchema table)
        {
            ShowTableSchema(table, true);
        }

        public void ShowTableSchema(TableSchema table,bool bShowAttrType)
        {
            lvwTable.Columns.Clear();
            lvwTable.Items.Clear();
            
            lblCondition.Text = "";
            lblCondition.Visible = false;
            
            lblTableName.Text = table.TableName;

            int WIDTH_PER_TEXT = 7;
            foreach(Field f in table.Fields)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = f.AttributeName;
                int length1 = f.AttributeName.Length * WIDTH_PER_TEXT + 10;
                int length2 = f.AttributeType.ToString().Length * WIDTH_PER_TEXT + 10;
                if (bShowAttrType)
                    header.Width = length1 > length2 ? length1 : length2;
                else
                    header.Width = length1;

                lvwTable.Columns.Add(header);
            }

            if (table.Fields.Count>0 && bShowAttrType)
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

            CurrentSite = null;
        }

        public void SetEmptyItem(int count)
        {
            lvwTable.GridLines = true;
            lvwTable.Scrollable = false;
            for (int j = 0; j < count; j++)
            {
                ListViewItem item = new ListViewItem();
                for (int i = 1; i < lvwTable.Columns.Count; i++)
                {
                    ListViewItem.ListViewSubItem sub = new ListViewItem.ListViewSubItem();
                    item.SubItems.Add(sub);
                }
                lvwTable.Items.Add(item);
            }
            TableUtility util = new TableUtility();
            //util.PaintCrossColor(lvwTable, Color.White, Color.LightGray);
            util.EqualColumnWidth(lvwTable, this.Size);
        }

        public void ClearItem()
        {
            lvwTable.Items.Clear();
        }

        

        public string Condition        
        {
            get{return lblCondition.Text;}
            set
            {

                lblCondition.Visible = (value != "");
                lblCondition.Text = value;
            }
        }



        
    }
}
