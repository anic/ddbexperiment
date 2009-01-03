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
using System.Drawing.Drawing2D;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscFragmentShower : UserControl
    {
        

        public UscFragmentShower()
        {
            InitializeComponent();
            
        }

        private TableSchema CloneSchema(TableSchema schema)
        {
            TableSchema result = new TableSchema();
            result.TableName = schema.TableName;
            foreach (Field field in schema.Fields)
            {
                Field f = new Field();
                f.AttributeName = " ";
                result.Fields.Add(f);
            }
            return result;
        }

        int y = 56;
        int HEIGHT_PER_LINE = 16;
        Fragment oldFragment;
        GlobalDirectory gdd;
        bool ignore = false;

        public void ShowFragment(Fragment fragment,GlobalDirectory gdd)
        {
            oldFragment = fragment;
            this.gdd = gdd;

            UscSchemaViewer schemaViewer = new UscSchemaViewer();
            schemaViewer.Width = this.Width;

            int rows = 0;
            int rowIndex = -1;
            int columns = 1;
            int colunmIndex = -1;

            Fragment parent = FindParentFragment(fragment, gdd);
            for (int i = 0; i < parent.Children.Count; i++)
                if (parent.Children[i].Name == fragment.Name)
                    rowIndex = i;

            while (parent != null && parent.Type != FragmentType.None) //根节点
            {
                rows = rows > parent.Children.Count ? rows : parent.Children.Count;
                
                Fragment cur = parent;
                parent = FindParentFragment(parent, gdd);

                if (parent.Type == FragmentType.None && cur.Type == FragmentType.Vertical)
                {
                    columns = parent.Children.Count;
                    for (int i = 0; i < parent.Children.Count; i++)
                        if (parent.Children[i].Name == fragment.Name)
                            colunmIndex = i;
                }
            }

            rows = rows > parent.Children.Count ? rows : parent.Children.Count;

            schemaViewer.Height = y + (rows + 1) * HEIGHT_PER_LINE;
            TableSchema cloneSchema = CloneSchema(fragment.LogicSchema);
            schemaViewer.ShowTableSchema(cloneSchema, false);

            schemaViewer.ClearItem();
            schemaViewer.SetEmptyItem(rows);

            Bitmap image = new Bitmap(this.Width, this.Height);
            Rectangle rect = schemaViewer.ClientRectangle;
            rect.Width += 5;
            rect.Height +=5;
            schemaViewer.DrawToBitmap(image, new Rectangle(0,0,image.Width,image.Height));

            DrawFragments(image, fragment, gdd, rows, rowIndex, columns, colunmIndex, schemaViewer.Width - 20);

            ignore = true;
            this.pcbImage.Width = image.Width;
            this.pcbImage.Height = image.Height;
            this.pcbImage.Image = image;
            ignore = false;

            Invalidate();
        }

        private Fragment FindParentFragment(Fragment fragment, GlobalDirectory gdd)
        {
            string name = fragment.Name;
            int index = name.LastIndexOf('.');
            if (index != -1)
                name = name.Substring(0, index);

            return gdd.Fragments.GetFragmentByName(name);

        }

        private void DrawFragments(Image image,Fragment f,GlobalDirectory gdd,int rows,int rowsIndex,int columns,int columnIndex,int width)
        {
            Color[] colors = new Color[] {Color.Red, Color.Green, Color.Orange, Color.Blue };
            Graphics g = Graphics.FromImage(image);

            if (columns > 1)
            {
                for (int i = 0; i < columns-1; i++)
                {
                    Rectangle rect = new Rectangle(10 + i * width / columns, y, width / columns, HEIGHT_PER_LINE * rows);
                    if (i == columnIndex)
                        DrawFragment(g, rect, colors[i % colors.Length], 255);
                    else
                        DrawFragment(g, rect, colors[i % colors.Length], 64);
                }
            }

            for (int i = 0; i < rows; i++)
            {
                Rectangle rect = new Rectangle(10 + (columns - 1) * width / columns, y + i * (HEIGHT_PER_LINE + 2), width / columns, HEIGHT_PER_LINE);
                if (i == rowsIndex)
                    DrawFragment(g, rect, colors[i % colors.Length], 255);
                else
                    DrawFragment(g, rect, colors[i % colors.Length], 64);
            }
            
        }

        private void DrawFragment(Graphics g,Rectangle rect, Color color, int alpha)
        {
            Brush brush = new SolidBrush(Color.FromArgb(alpha, color));
            Pen pen = new Pen(color, 1);
            FillRoundRectangle(g, brush, rect, 5);
            DrawRoundRectangle(g, pen, rect, 5);
        }

        internal static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            if (rect.X + cornerRadius < rect.Right - cornerRadius * 2)
                roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            if (rect.Y + cornerRadius * 2 < rect.Y + rect.Height - cornerRadius * 2)
                roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            if (rect.Right - cornerRadius * 2 > rect.X + cornerRadius * 2)
                roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            if (rect.Bottom - cornerRadius * 2 > rect.Y + cornerRadius * 2)
                roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        public static void DrawRoundRectangle(Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }
        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        private void pcbImage_Resize(object sender, EventArgs e)
        {
            if (ignore)
                return;

            if (oldFragment != null && gdd != null)
                ShowFragment(this.oldFragment, gdd);
        }

        
        
    }
}
