using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace DistDBMS.UserInterface.Controls.SqlInput.GListBox
{
    //GListBox 类 
    public class GListBox : ListBox
    {
        private ImageList m_ImageList;

        public ImageList ImageList { get { return m_ImageList; } set { m_ImageList = value; } }

        private int m_LevelWidth;
        public int LevelWidth {
            get
            {
                return m_LevelWidth;
            }
            set
            {
                if (value > 0)
                    m_LevelWidth = value;
            }
        }

        public GListBox() { 
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.LevelWidth = 10;
            this.DoubleBuffered = true;
        }

        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground(); 
            e.DrawFocusRectangle();

            GListBoxItem item; 
            Rectangle bounds = e.Bounds;
            
            try
            {
                item = (GListBoxItem)Items[e.Index];
                if (item != null)
                {
                    int left = bounds.Left + item.Level * LevelWidth;
                    if (item.ImageIndex != -1 && m_ImageList != null && m_ImageList.Images.Count > item.ImageIndex)
                    {
                        Size imageSize = m_ImageList.ImageSize;
                        ImageList.Draw(e.Graphics, left, bounds.Top, item.ImageIndex);
                        left += imageSize.Width;
                    }
                    e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor), left, bounds.Top + (bounds.Height - e.Font.Height) / 2);

                }
            }
            catch
            {
                if (e.Index != -1 && Items.Count> e.Index)
                {
                    e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(Text, e.Font, new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                }
            }
            base.OnDrawItem(e);
        }
    }

}
