﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Dictionary;
using DistDBMS.UserInterface.Controls.SqlInput;
using DistDBMS.Common.Table;

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

        public bool EnableTip
        {
            get { return sqlTextBox.ShowTip; }
            set { sqlTextBox.ShowTip = value; }
        }

        private void menuItemColorKeyword_Click(object sender, EventArgs e)
        {
            sqlTextBox.ColorKeyword = menuItemColorKeyword.Checked;
        }

        private void menuItemColorTable_Click(object sender, EventArgs e)
        {
            sqlTextBox.ColorTable = menuItemColorTable.Checked;
        }

        private void btnExecuteSQL_Click(object sender, EventArgs e)
        {
            if (OnExecuteSQL != null)
                OnExecuteSQL(sender, e);
        }

        public string SQLText
        {
            get
            {
                return sqlTextBox.Text;
            }
            set { 
                sqlTextBox.Text = value;
                
            }
        }

        public string CommandResultText {
            get { return txtResult.Text; }
            set { txtResult.Text = value;
            
            txtResult.Select(txtResult.Text.Length, 0);
            txtResult.ScrollToCaret();
            }
        }

        public void AddCommandResult(string result)
        {
            txtResult.Text += result + "\r\n";
            
            txtResult.Select(txtResult.Text.Length, 0);
            txtResult.ScrollToCaret();
        }

        public void SetResultTable(Table table)
        {
            uscTableViewer1.Table = table;
        }

        public event EventHandler OnExecuteSQL;
        

        
    }
}
