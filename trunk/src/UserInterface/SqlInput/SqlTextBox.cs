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
        string[] symbol = new string[] { ",", ".", ";" };
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


        protected override void OnKeyDown(KeyEventArgs e)
        {
            
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
                    frmTip.Visible = false;
                    break;
                case Keys.Enter:
                    if (frmTip.Visible && frmTip.IsTipSelected)
                    {
                        int lastPosition = this.SelectionStart;
                        int index = Text.Substring(0, this.SelectionStart).LastIndexOf(' ');
                        if (index >= 0)
                        {
                            int right = this.Text.IndexOf(' ', index + 1);
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

                        frmTip.Visible = false;
                        
                        e.Handled = true;
                    }
                    break;
            }

            if (!e.Handled)
                base.OnKeyUp(e);
        }

        private string LastWord
        {
            get
            {

                int index = Text.Substring(0, this.SelectionStart).LastIndexOf(' ');
                if (index >= 0)
                {
                    int right = this.Text.IndexOf(' ', index + 1);
                    if (right >= 0)
                        return Text.Substring(index + 1, right - index - 1);
                    else
                        return Text.Substring(index + 1);
                }
                else
                    return Text;
            }
        }

        private void FillColor()
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

            frmTip.MatchTip(LastWord);
            
            if (LastWord!="")
            {
                if (!frmTip.Visible)
                    frmTip.Show(this);
            }
            

            this.Focus();
            FillColor();

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
