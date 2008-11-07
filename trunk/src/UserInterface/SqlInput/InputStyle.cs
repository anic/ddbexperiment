using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DistDBMS.UserInterface.SqlInput
{
    public class InputStyle
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// 关键字颜色
        /// </summary>
        public Color KeywordColor { get; set; }

        /// <summary>
        /// 高亮颜色
        /// </summary>
        public List<Color> HighLightColors { get { return colors; } }
        List<Color> colors;

        public InputStyle()
        {
            BackgroundColor = Color.White;
            ForeColor = Color.Black;
            KeywordColor = Color.Blue;
            colors = new List<Color>();
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Orange);
        }
    }
}
