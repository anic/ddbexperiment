using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistDBMS.Common.Dictionary;
using System.Windows.Forms;
using System.Drawing;
using DistDBMS.UserInterface.Properties;

namespace DistDBMS.UserInterface.SqlInput
{
    class SqlTextBox:RichTextBox
    {
        FrmTip frmTip;
        string[] keywords = new string[] { "select", "from", "where" };
        string[] symbol = new string[] { " ", ",", ";" };
        public SqlTextBox()
            : base()
        {
            SetStyle();
            frmTip = new FrmTip();

            foreach (string s in keywords)
                frmTip.CreateKeyword(s);
        }



        GlobalDirectory gdd;
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
            }
        }

        InputStyle style = new InputStyle();
        public InputStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

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
        private void FillKeywordColor()
        {
            int lastSelectionPos = this.SelectionStart;
            int lastLength = this.SelectionLength;

            this.SelectionStart = 0;
            this.SelectionLength = this.Text.Length;
            this.SelectionColor = this.style.ForeColor;

            
            foreach (string key in keywords)
            {
                string str = this.Text;
                int left = str.IndexOf(key);
                int addition = 0;
                while (left >= 0)
                {
                    this.SelectionStart = left + addition;
                    this.SelectionLength = key.Length;
                    this.SelectionColor = style.KeywordColor;

                    addition = left + key.Length;
                    str = str.Substring(left + key.Length);
                    left = str.IndexOf(key);
                }
            }

            this.SelectionStart = lastSelectionPos;
            this.SelectionLength = lastLength;
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
                    if (!frmTip.Visible)
                        frmTip.Show(this);
                }
                else
                    frmTip.Visible = false;
            }
            

            this.Focus();

            //填充关键字颜色
            FillKeywordColor();

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
