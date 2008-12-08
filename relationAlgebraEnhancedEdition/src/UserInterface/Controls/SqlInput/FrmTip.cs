using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.UserInterface.Controls.SqlInput.GListBox;
using DistDBMS.UserInterface.Properties;
using System.Text.RegularExpressions;

namespace DistDBMS.UserInterface.Controls.SqlInput
{
    public partial class FrmTip : Form
    {
        public FrmTip()
        {
            InitializeComponent();

            imageList.Images.Add(Resources.img_column);
            imageList.Images.Add(Resources.img_table);
            imageList.Images.Add(Resources.img_keyword);


            
        }

        public void CreateKeyword(string keyword)
        {
            GListBoxItem item = new GListBoxItem();
            item.Text = keyword;
            item.ImageIndex = 2;
            lsbTip.Items.Add(item);
        }

        public void InitTips(GlobalDirectory gdd)
        {
            if (gdd == null)
                return;
            foreach (TableSchema t in gdd.Schemas)
            {
                GListBoxItem item = new GListBoxItem();
                item.Text = t.TableName;
                item.Tag = t;
                item.ImageIndex = 1;
                lsbTip.Items.Add(item);

                foreach (Field f in t.Fields)
                {
                    GListBoxItem subItem = new GListBoxItem();
                    subItem.Text = f.ToString();
                    subItem.Tag = f;
                    subItem.ImageIndex = 0;
                    subItem.Level = 1;
                    lsbTip.Items.Add(subItem);
                }

                
            }
        }

        public bool IsTipSelected 
        {
            get
            {
                return lsbTip.SelectedIndex >= 0;
            }
        }

        public string SelectedTip
        {
            get
            {
                if (IsTipSelected)
                    return (lsbTip.SelectedItem as GListBoxItem).Text;
                else
                    return "";
            }
        }

        public void MatchTip(string prefix)
        {
            if (prefix != "")
            {
                try
                {
                    for (int i = 0; i < lsbTip.Items.Count; i++)
                    {
                        GListBoxItem item = lsbTip.Items[i] as GListBoxItem;
                        Regex reg = new Regex(prefix, RegexOptions.IgnoreCase);
                        if (reg.Match(item.Text).Success)
                        {
                            lsbTip.SelectedIndex = i;
                            return;
                        }
                    }
                }
                catch
                { 

                }
            }
            lsbTip.SelectedIndex = -1;
        }

        public bool IsFullyMatch(string text)
        {
            if (lsbTip.SelectedIndex >=0 )
            {
                string str = (lsbTip.SelectedItem as GListBoxItem).Text;
                if (str.Equals(text,StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public void NextTip(int delta)
        {
            //没有提示
            if (lsbTip.Items.Count == 0)
                return;

            //没有选中
            if (lsbTip.SelectedIndex == -1)
                lsbTip.SelectedIndex = 0;
            else
            {
                //选中下一个
                int length = lsbTip.Items.Count;
                lsbTip.SelectedIndex = (lsbTip.SelectedIndices[0] + delta + length) % length;
                
            }
        }
    }
}
