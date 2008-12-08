namespace DistDBMS.UserInterface.Controls
{
    partial class UscExecuteQuery
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DistDBMS.UserInterface.Controls.SqlInput.InputStyle inputStyle2 = new DistDBMS.UserInterface.Controls.SqlInput.InputStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbp = new System.Windows.Forms.TabPage();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.uscTableViewer1 = new DistDBMS.UserInterface.Controls.UscTableViewer();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pnlOptTree = new System.Windows.Forms.Panel();
            this.optQTreeViewer = new DistDBMS.UserInterface.Controls.UscQTreeViewer();
            this.pnlRawTree = new System.Windows.Forms.Panel();
            this.rawQTreeViewer = new DistDBMS.UserInterface.Controls.UscQTreeViewer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnExecuteSQL = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmbColorStyle = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.menuItemShowTip = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemColorKeyword = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemColorTable = new System.Windows.Forms.ToolStripMenuItem();
            this.sqlTextBox = new DistDBMS.UserInterface.Controls.SqlInput.SqlTextBox();
            this.tabControl1.SuspendLayout();
            this.tbp.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pnlOptTree.SuspendLayout();
            this.pnlRawTree.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbp);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(10, 182);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(403, 109);
            this.tabControl1.TabIndex = 1;
            // 
            // tbp
            // 
            this.tbp.Controls.Add(this.txtResult);
            this.tbp.Location = new System.Drawing.Point(4, 22);
            this.tbp.Name = "tbp";
            this.tbp.Padding = new System.Windows.Forms.Padding(3);
            this.tbp.Size = new System.Drawing.Size(395, 83);
            this.tbp.TabIndex = 0;
            this.tbp.Text = "控制台显示";
            this.tbp.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(389, 77);
            this.txtResult.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.uscTableViewer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(395, 83);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "表格显示";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // uscTableViewer1
            // 
            this.uscTableViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscTableViewer1.Location = new System.Drawing.Point(3, 3);
            this.uscTableViewer1.Name = "uscTableViewer1";
            this.uscTableViewer1.Size = new System.Drawing.Size(389, 77);
            this.uscTableViewer1.TabIndex = 0;
            this.uscTableViewer1.Table = null;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitter2);
            this.tabPage1.Controls.Add(this.pnlOptTree);
            this.tabPage1.Controls.Add(this.pnlRawTree);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(395, 83);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "查询树";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.Control;
            this.splitter2.Location = new System.Drawing.Point(196, 3);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 77);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // pnlOptTree
            // 
            this.pnlOptTree.BackColor = System.Drawing.SystemColors.Control;
            this.pnlOptTree.Controls.Add(this.optQTreeViewer);
            this.pnlOptTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptTree.Location = new System.Drawing.Point(196, 3);
            this.pnlOptTree.Name = "pnlOptTree";
            this.pnlOptTree.Padding = new System.Windows.Forms.Padding(10);
            this.pnlOptTree.Size = new System.Drawing.Size(196, 77);
            this.pnlOptTree.TabIndex = 1;
            // 
            // optQTreeViewer
            // 
            this.optQTreeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optQTreeViewer.Location = new System.Drawing.Point(10, 10);
            this.optQTreeViewer.Name = "optQTreeViewer";
            this.optQTreeViewer.Size = new System.Drawing.Size(176, 57);
            this.optQTreeViewer.TabIndex = 0;
            // 
            // pnlRawTree
            // 
            this.pnlRawTree.BackColor = System.Drawing.SystemColors.Control;
            this.pnlRawTree.Controls.Add(this.rawQTreeViewer);
            this.pnlRawTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRawTree.Location = new System.Drawing.Point(3, 3);
            this.pnlRawTree.Name = "pnlRawTree";
            this.pnlRawTree.Padding = new System.Windows.Forms.Padding(10);
            this.pnlRawTree.Size = new System.Drawing.Size(193, 77);
            this.pnlRawTree.TabIndex = 0;
            // 
            // rawQTreeViewer
            // 
            this.rawQTreeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rawQTreeViewer.Location = new System.Drawing.Point(10, 10);
            this.rawQTreeViewer.Name = "rawQTreeViewer";
            this.rawQTreeViewer.Size = new System.Drawing.Size(173, 57);
            this.rawQTreeViewer.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(10, 179);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(403, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExecuteSQL,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.cmbColorStyle,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(10, 10);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(403, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnExecuteSQL
            // 
            this.btnExecuteSQL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExecuteSQL.Image = global::DistDBMS.UserInterface.Properties.Resources.img_exesql;
            this.btnExecuteSQL.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExecuteSQL.Name = "btnExecuteSQL";
            this.btnExecuteSQL.Size = new System.Drawing.Size(23, 22);
            this.btnExecuteSQL.Text = "执行SQL";
            this.btnExecuteSQL.Click += new System.EventHandler(this.btnExecuteSQL_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel1.Text = "颜色方案";
            // 
            // cmbColorStyle
            // 
            this.cmbColorStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColorStyle.Name = "cmbColorStyle";
            this.cmbColorStyle.Size = new System.Drawing.Size(121, 25);
            this.cmbColorStyle.SelectedIndexChanged += new System.EventHandler(this.cmbColorStyle_SelectedIndexChanged);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShowTip,
            this.menuItemColorKeyword,
            this.menuItemColorTable});
            this.toolStripSplitButton1.Image = global::DistDBMS.UserInterface.Properties.Resources.img_tool;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 22);
            this.toolStripSplitButton1.Text = "工具";
            // 
            // menuItemShowTip
            // 
            this.menuItemShowTip.CheckOnClick = true;
            this.menuItemShowTip.Name = "menuItemShowTip";
            this.menuItemShowTip.Size = new System.Drawing.Size(136, 22);
            this.menuItemShowTip.Text = "显示提示";
            this.menuItemShowTip.Click += new System.EventHandler(this.menuItemShowTip_Click);
            // 
            // menuItemColorKeyword
            // 
            this.menuItemColorKeyword.CheckOnClick = true;
            this.menuItemColorKeyword.Image = global::DistDBMS.UserInterface.Properties.Resources.img_keyword;
            this.menuItemColorKeyword.Name = "menuItemColorKeyword";
            this.menuItemColorKeyword.Size = new System.Drawing.Size(136, 22);
            this.menuItemColorKeyword.Text = "关键字高亮";
            this.menuItemColorKeyword.Click += new System.EventHandler(this.menuItemColorKeyword_Click);
            // 
            // menuItemColorTable
            // 
            this.menuItemColorTable.CheckOnClick = true;
            this.menuItemColorTable.Image = global::DistDBMS.UserInterface.Properties.Resources.img_colortable;
            this.menuItemColorTable.Name = "menuItemColorTable";
            this.menuItemColorTable.Size = new System.Drawing.Size(136, 22);
            this.menuItemColorTable.Text = "彩色表格";
            this.menuItemColorTable.Click += new System.EventHandler(this.menuItemColorTable_Click);
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.BackColor = System.Drawing.Color.White;
            this.sqlTextBox.ColorKeyword = true;
            this.sqlTextBox.ColorTable = true;
            this.sqlTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.sqlTextBox.ForeColor = System.Drawing.Color.Black;
            this.sqlTextBox.GDD = null;
            this.sqlTextBox.Location = new System.Drawing.Point(10, 35);
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.ShowTip = true;
            this.sqlTextBox.Size = new System.Drawing.Size(403, 144);
            inputStyle2.BackgroundColor = System.Drawing.Color.White;
            inputStyle2.ForeColor = System.Drawing.Color.Black;
            inputStyle2.KeywordColor = System.Drawing.Color.Blue;
            inputStyle2.Name = "";
            this.sqlTextBox.Style = inputStyle2;
            this.sqlTextBox.TabIndex = 0;
            this.sqlTextBox.Text = "";
            // 
            // UscExecuteQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.sqlTextBox);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UscExecuteQuery";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(423, 301);
            this.SizeChanged += new System.EventHandler(this.UscExecuteQuery_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tbp.ResumeLayout(false);
            this.tbp.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.pnlOptTree.ResumeLayout(false);
            this.pnlRawTree.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DistDBMS.UserInterface.Controls.SqlInput.SqlTextBox sqlTextBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabPage tbp;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmbColorStyle;
        private System.Windows.Forms.ToolStripButton btnExecuteSQL;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowTip;
        private System.Windows.Forms.ToolStripMenuItem menuItemColorKeyword;
        private System.Windows.Forms.ToolStripMenuItem menuItemColorTable;
        private System.Windows.Forms.TextBox txtResult;
        private UscTableViewer uscTableViewer1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel pnlOptTree;
        private System.Windows.Forms.Panel pnlRawTree;
        private UscQTreeViewer rawQTreeViewer;
        private UscQTreeViewer optQTreeViewer;
        private System.Windows.Forms.Splitter splitter2;
    }
}
