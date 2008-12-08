using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DistDBMS.UserInterface.Controls.SqlInput
{
    public class InputStyle
    {
        public string Name { get; set; }

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
            Name = "";
        }


        public override string ToString()
        {
            return Name;
        }

        static InputStyle blackStyle;
        public static InputStyle BlackStyle
        {
            get
            { 
                if (blackStyle==null)
                {
                    blackStyle = new InputStyle();
                    blackStyle.BackgroundColor = Color.Black;
                    blackStyle.ForeColor = Color.White;
                    blackStyle.HighLightColors.Add(Color.Red);
                    blackStyle.HighLightColors.Add(Color.Yellow);
                    blackStyle.HighLightColors.Add(Color.Green);
                    blackStyle.HighLightColors.Add(Color.Pink);
                    blackStyle.KeywordColor = Color.Blue;
                    blackStyle.Name = "黑底白字";
                }
                return blackStyle;
            }
        }

        static InputStyle whiteStyle;
        public static InputStyle WhiteStyle
        {
            get
            {
                if (whiteStyle == null)
                {
                    whiteStyle = new InputStyle();
                    whiteStyle.HighLightColors.AddRange(new Color[] { Color.Red, Color.Orange, Color.Green, Color.Purple });
                    whiteStyle.Name = "白底黑字";
                }
                return whiteStyle;
            }
        }
    }
}
