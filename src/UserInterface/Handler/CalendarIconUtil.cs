using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Collections;

namespace DistDBMS.UserInterface.Handler
{

    /// <summary>
    /// 生成Icon的类
    /// </summary>
    class IconUtil
    {
        static IconUtil instance = null;
        public static IconUtil Instance
        {
            get
            {
                if (instance == null)
                    instance = new IconUtil();
                return instance;

            }
        }

        Pen pen;
        Hashtable imgList;
        protected IconUtil()
        {
            pen = new Pen(Color.Empty); 
            imgList = new Hashtable();
            
        }

        public Image DrawIconImage(Color color)
        {
            object result = imgList[color];
            if (result != null)
                return result as Image;

            const int inner_width = 13;
            const int inner_height = 13;
            const int heightList = 0;
            const int height = 17;
            const int width = 17;
            Image img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);
            
            Rectangle destRect1;
            destRect1 = new Rectangle(1 + (width - inner_width) / 2, 1 + heightList + ((height - inner_height) / 2), inner_width - 2, inner_height - 2);
            //绘制方形
            g.FillRectangle(new SolidBrush(color), destRect1);
            Rectangle rect1 = new Rectangle((width - inner_width) / 2, heightList + ((height - inner_height) / 2), inner_width, inner_height);
            Rectangle rect2 = rect1;
            rect2.Y = rect1.Y + 1;
            rect2.Height = (rect1.Height / 2) - 2;
            //绘制上斜面阴影
            DrawFillVGradient(g, rect2, Color.FromArgb(183, 255, 255, 255), Color.FromArgb(80, 255, 255, 255));
            Rectangle rect3 = rect2;
            rect3.Y = rect2.Bottom + 1;
            rect3.Height = rect1.Height - rect2.Height - 3;
            //绘制下斜面阴影
            DrawFillVGradient(g, rect3, Color.FromArgb(80, 255, 255, 255), Color.FromArgb(177, 255, 255, 255));

            //绘制边
            pen.Color = color;
            g.DrawLine(pen, rect1.Left + 1, rect1.Top, rect1.Right - 2, rect1.Top);
            g.DrawLine(pen, rect1.Left + 1, rect1.Bottom - 1, rect1.Right - 2, rect1.Bottom - 1);
            g.DrawLine(pen, rect1.Left, rect1.Top + 1, rect1.Left, rect1.Bottom - 2);
            g.DrawLine(pen, rect1.Right - 1, rect1.Top + 1, rect1.Right - 1, rect1.Bottom - 2);


            imgList[color] = img;

            return img;
        }

        private void DrawFillVGradient(Graphics g_, Rectangle rect1, Color color1, Color color2)
        {
            System.Int32 r, g, b, a;
            for (int i = 0; i < rect1.Height; i++)
            {
                a = color2.A + ((((rect1.Height - 1) - i) * (color1.A - color2.A)) / (rect1.Height - 1));
                r = color2.R + ((((rect1.Height - 1) - i) * (color1.R - color2.R)) / (rect1.Height - 1));
                g = color2.G + ((((rect1.Height - 1) - i) * (color1.G - color2.G)) / (rect1.Height - 1));
                b = color2.B + ((((rect1.Height - 1) - i) * (color1.B - color2.B)) / (rect1.Height - 1));
                pen.Color = Color.FromArgb(a, r, g, b);
                g_.DrawLine(pen, rect1.Left, rect1.Top + i, rect1.Right - 1, rect1.Top + i);
            }
        }
    }
}
