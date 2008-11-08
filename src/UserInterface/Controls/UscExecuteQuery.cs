using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.UserInterface.Controls.SqlInput;

namespace DistDBMS.UserInterface.Controls
{
    public partial class UscExecuteQuery : UserControl
    {
        InputStyle[] styleList = new InputStyle[] { InputStyle.WhiteStyle, InputStyle.BlackStyle };
        public UscExecuteQuery()
        {
            InitializeComponent();
            foreach (InputStyle style in styleList)
                cmbColorStyle.Items.Add(style);

            sqlTextBox.ColorTable = true;
            sqlTextBox.ColorKeyword = true;
            sqlTextBox.ShowTip = true;

            cmbColorStyle.SelectedIndex = 0;
            menuItemColorTable.Checked = true;
            menuItemColorKeyword.Checked = true;
            menuItemShowTip.Checked = true;
            
        }

        
        public void SetGlobalDirectory(GlobalDirectory gdd)
        {
            sqlTextBox.GDD = gdd;
        }

        
        private void cmbColorStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            sqlTextBox.Style = cmbColorStyle.SelectedItem as InputStyle;
        }

        private void menuItemShowTip_Click(object sender, EventArgs e)
        {
            sqlTextBox.ShowTip = menuItemShowTip.Checked;
        }

        private void menuItemColorKeyword_Click(object sender, EventArgs e)
        {
            sqlTextBox.ColorKeyword = menuItemColorKeyword.Checked;
        }

        private void menuItemColorTable_Click(object sender, EventArgs e)
        {
            sqlTextBox.ColorTable = menuItemColorTable.Checked;
        }

        

        
    }
}
