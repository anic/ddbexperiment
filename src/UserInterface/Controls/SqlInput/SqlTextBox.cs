using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistDBMS.Common.Dictionary;
using System.Windows.Forms;
using System.Drawing;
using DistDBMS.UserInterface.Properties;
using DistDBMS.Common.Table;

namespace DistDBMS.UserInterface.Controls.SqlInput
{
    class SqlTextBox:RichTextBox
    {
        FrmTip frmTip;
        string[] keywords = new string[] { "select", "from", "where", "delete", "insert" };
        string[] symbol = new string[] { " ", ",", ";" };

        List<string> table = new List<string>();

        public SqlTextBox()
            : base()
        {
            SetStyle();
            frmTip = new FrmTip();

            foreach (string s in keywords)
                frmTip.CreateKeyword(s);
        }

        /// <summary>
        /// 是否高亮关键字
        /// </summary>
        public bool ColorKeyword {
            get { return bColorKeyword; }
            set { 
                bColorKeyword = value;
                FillTextColor();
                }
        }
        bool bColorKeyword = true;

        /// <summary>
        /// 是否填充表格颜色
        /// </summary>
        public bool ColorTable {
            get { return bColorTable; }
            set { 
                bColorTable = value;
                FillTextColor();
            }
        }
        bool bColorTable = true;

        /// <summary>
        /// 是否显示提示
        /// </summary>
        public bool ShowTip
        {
            get { return bShowTip; }
            set { 
                bShowTip = value; 
                frmTip.Visible = false; 
            }
        }
        bool bShowTip = true;

        GlobalDirectory gdd;
        /// <summary>
        /// 全局数据字典
        /// </summary>
        public GlobalDirectory GDD
        {
            get
            {
                return gdd;
            }
            set
            {
                gdd = value;
                frmTip.InitTips(gdd);
                if (gdd != null)
                {
                    table.Clear();
                    foreach (TableSchema t in gdd.Schemas)
                        table.Add(t.TableName);
                }

            }
        }

        /// <summary>
        /// 风格
        /// </summary>
        public InputStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                SetStyle();
                FillTextColor();
            }
        }
        InputStyle style = new InputStyle();

        private void SetStyle()
        {
            if (style != null)
            {
                this.BackColor = style.BackgroundColor;
                this.ForeColor = style.ForeColor;
            }
        }

        Keys lastKey = Keys.None;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            lastKey = e.KeyData;
            switch (e.KeyData)
            { 
                case Keys.Up:
                    if (frmTip.Visible)
                    {
                        frmTip.NextTip(-1);
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (frmTip.Visible)
                    {
                        frmTip.NextTip(1);
                        e.Handled = true;
                    }
                    break;
                case Keys.Escape:
                case Keys.Back:
                case Keys.Right:
                case Keys.Left:
                    frmTip.Visible = false;
                    break;
                case Keys.Enter:
                    if (frmTip.Visible)
                    {
                        if (frmTip.IsTipSelected)
                        {
                            int lastPosition = this.SelectionStart;
                            int end = this.SelectionStart > 0 ? this.SelectionStart - 1 : 0;
                            int index = LastIndexOfSymbol(Text, 0, end);
                            if (index >= 0)
                            {
                                int right = FirstIndexOfSymbol(Text, index + 1, Text.Length - 1);
                                if (right >= 0)
                                {
                                    int length = frmTip.SelectedTip.Length;
                                    this.Text = this.Text.Substring(0, index + 1) + frmTip.SelectedTip + this.Text.Substring(right);
                                    this.SelectionStart = index + length + 1;
                                }
                                else
                                {
                                    this.Text = this.Text.Substring(0, index + 1) + frmTip.SelectedTip;
                                    this.SelectionStart = this.Text.Length;
                                }
                            }
                            else
                            {
                                this.Text = frmTip.SelectedTip;
                                this.SelectionStart = this.Text.Length;
                            }

                            e.Handled = true;
                        }
                    }


                    break;
            }

            if (!e.Handled)
                base.OnKeyUp(e);
        }

        /// <summary>
        /// 查找最后的符号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int LastIndexOfSymbol(string str,int startIndex,int endIndex)
        {
            int result, left;
            result = left = -1;
            string str1 = str.Substring(startIndex, endIndex - startIndex + 1);
            foreach (string s in symbol)
            {
                left = str1.LastIndexOf(s, StringComparison.CurrentCultureIgnoreCase);
                if (left > result)
                    result = left;
            }
            return (result == -1) ? -1 : (result + startIndex);
        }

        /// <summary>
        /// 查找最前的符号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int FirstIndexOfSymbol(string str,int startIndex,int endIndex)
        {
            int result, right;
            result = right = -1;
            string str1 = str.Substring(startIndex, endIndex - startIndex + 1);
            foreach (string s in symbol)
            {
                right = str1.IndexOf(s, StringComparison.CurrentCultureIgnoreCase);

                if (result == -1 && right != -1)
                    result = right;

                if (right != -1 && right < result)
                    result = right;
            }
            return (result == -1) ? -1 : (result + startIndex);
        }

        /// <summary>
        /// 当前最后一个字
        /// </summary>
        private string LastWord
        {
            get
            {
                int end = this.SelectionStart > 0 ? this.SelectionStart - 1 : 0;
                int index = LastIndexOfSymbol(Text, 0, end);
                if (index >= 0)
                {
                    int right = FirstIndexOfSymbol(Text, index + 1, Text.Length - 1);
                    if (right >= 0)
                        return Text.Substring(index + 1, right - index - 1).Trim();
                    else
                        return Text.Substring(index + 1).Trim();
                }
                else
                    return Text.Trim();
            }
        }

        /// <summary>
        /// 填入关键字颜色
        /// </summary>
        private void FillTextColor()
        {
            int lastSelectionPos = this.SelectionStart;
            int lastLength = this.SelectionLength;

            this.SelectionStart = 0;
            this.SelectionLength = this.Text.Length;
            this.SelectionColor = this.style.ForeColor;

            if (ColorKeyword)
            {
                foreach (string key in keywords)
                    MatchText(key, style.KeywordColor);
            }

            if (ColorTable)
            {
                int len = style.HighLightColors.Count;
                if (len > 0)
                {
                    for (int i = 0; i < table.Count; i++)
                    {
                        string tablename = table[i];
                        MatchText(tablename, style.HighLightColors[i % len]);
                    }
                }
            }

            this.SelectionStart = lastSelectionPos;
            this.SelectionLength = lastLength;
        }

        /// <summary>
        /// 匹配字并填充颜色
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        private void MatchText(string text, Color color)
        {
            string str = this.Text;
            int left = str.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
            int addition = 0;
            while (left >= 0)
            {
                this.SelectionStart = left + addition;
                this.SelectionLength = text.Length;

                this.SelectionColor = color;

                addition += left + text.Length;
                str = Text.Substring(addition);
                left = str.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (Text.Length == 0)
            {
                base.OnTextChanged(e);
                return;
            }

            Point p = GetPositionFromCharIndex(Text.Length - 1);
            p.X -= 20;
            p.Y += 20;

            frmTip.Location = this.PointToScreen(p);

            string lastWord = LastWord;

            
            if (lastKey == Keys.Enter)  //按了回车键
            {
                frmTip.Visible = false;
            }
            else
            {
                if (frmTip.Visible) //如果是其他键，如果提示框可见，匹配
                    frmTip.MatchTip(lastWord);


                if (frmTip.IsFullyMatch(lastWord)) //如果完全匹配，则隐藏提示框
                {
                    frmTip.Visible = false;
                }
                else if (lastWord != "") //如果不完全匹配，且不为空“”，显示提示框
                {
                    if (!frmTip.Visible && ShowTip)
                        frmTip.Show(this);
                }
                else
                    frmTip.Visible = false;
            }
            

            this.Focus();

            //填充关键字颜色
            FillTextColor();

            base.OnTextChanged(e);
            
            
            
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ResumeLayout(false);

        }

        private ImageList imageList;
        private System.ComponentModel.IContainer components;

    }
}
