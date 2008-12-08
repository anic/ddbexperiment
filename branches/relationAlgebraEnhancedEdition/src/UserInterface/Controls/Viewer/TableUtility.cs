using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DistDBMS.UserInterface.Controls
{
    class TableUtility
    {
        

        public void PaintColor(ListView table, Color odd, Color even)
        {
            for (int i = 0; i < table.Items.Count; i++)
                if (i % 2 == 0)
                    FillColor(table.Items[i], even);
                else
                    FillColor(table.Items[i], odd);
        }

        public void PaintCrossColor(ListView table, Color odd, Color even)
        {
            for (int i = 0; i < table.Items.Count; i++)
                if (i % 2 == 0)
                    FillCrossColor(table.Items[i], even, odd);
                else
                    FillCrossColor(table.Items[i], odd, even);
        }

        private void FillCrossColor(ListViewItem item, Color odd, Color even)
        {
            item.BackColor = odd;
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                if (i % 2 == 0)
                    item.SubItems[i].BackColor = even;
                else
                    item.SubItems[i].BackColor = odd;
            }
        }

        private void FillColor(ListViewItem item, Color bgColor)
        {
            item.BackColor = bgColor;
            foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                sub.BackColor = bgColor;
        }

        public void EqualColumnWidth(ListView table, Size size)
        {
            int width = size.Width / table.Columns.Count;
            for (int i = 0; i < table.Columns.Count; i++)
                table.Columns[i].Width = width;
        }

        public void FitSize(ListView table, Size size)
        {
            int width = size.Width;
            for (int i = 0; i < table.Columns.Count; i++)
                width -= table.Columns[i].Width;

            if (width > 0 && table.Columns.Count > 0)
                table.Columns[table.Columns.Count - 1].Width += width - 5;
        }
    }
}
