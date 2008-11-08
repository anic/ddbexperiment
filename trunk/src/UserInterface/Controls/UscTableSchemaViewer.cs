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

            lblTableName.Text = table.TableName;
            
        }
    }
}
