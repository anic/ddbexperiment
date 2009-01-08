using System;
namespace DistDBMS.UserInterface
{
    partial class FrmApp
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("查询");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("站点");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("表");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("分片");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("数据字典", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmApp));
            this.tvwMenu = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsWizzard = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImportScript = new System.Windows.Forms.ToolStripMenuItem();
            this.tsImportData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportScript = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOption = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsCancelButton = new System.Windows.Forms.ToolStripSplitButton();
            this.tsExesqlProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tsInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.uscExecuteQuery = new DistDBMS.UserInterface.Controls.UscExecuteQuery();
            this.uscSiteViewer = new DistDBMS.UserInterface.Controls.UscSiteViewer();
            this.uscFragmentViewer = new DistDBMS.UserInterface.Controls.UscFragmentViewer();
            this.pnlControl.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvwMenu
            // 
            this.tvwMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwMenu.ImageIndex = 0;
            this.tvwMenu.ImageList = this.imageList;
            this.tvwMenu.ItemHeight = 23;
            this.tvwMenu.Location = new System.Drawing.Point(10, 10);
            this.tvwMenu.Name = "tvwMenu";
            treeNode1.ImageIndex = -2;
            treeNode1.Name = "Node0";
            treeNode1.Text = "查询";
            treeNode2.Name = "Node3";
            treeNode2.Text = "站点";
            treeNode3.Name = "Node4";
            treeNode3.Text = "表";
            treeNode4.Name = "Node6";
            treeNode4.Text = "分片";
            treeNode5.Name = "Node1";
            treeNode5.Text = "数据字典";
            this.tvwMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode5});
            this.tvwMenu.SelectedImageIndex = 0;
            this.tvwMenu.Size = new System.Drawing.Size(183, 415);
            this.tvwMenu.TabIndex = 2;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(20, 20);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(203, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 435);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.tvwMenu);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlControl.Location = new System.Drawing.Point(0, 25);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Padding = new System.Windows.Forms.Padding(10);
            this.pnlControl.Size = new System.Drawing.Size(203, 435);
            this.pnlControl.TabIndex = 5;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(697, 25);
            this.mainMenu.TabIndex = 7;
            this.mainMenu.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsWizzard,
            this.tsImportScript,
            this.tsImportData,
            this.toolStripSeparator1,
            this.tsExit});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // tsWizzard
            // 
            this.tsWizzard.Name = "tsWizzard";
            this.tsWizzard.Size = new System.Drawing.Size(160, 22);
            this.tsWizzard.Text = "运行向导";
            this.tsWizzard.Click += new System.EventHandler(this.tsWizzard_Click);
            // 
            // tsImportScript
            // 
            this.tsImportScript.Name = "tsImportScript";
            this.tsImportScript.Size = new System.Drawing.Size(160, 22);
            this.tsImportScript.Text = "导入初始化脚本";
            this.tsImportScript.Click += new System.EventHandler(this.tsImportScript_Click);
            // 
            // tsImportData
            // 
            this.tsImportData.Name = "tsImportData";
            this.tsImportData.Size = new System.Drawing.Size(160, 22);
            this.tsImportData.Text = "导入数据";
            this.tsImportData.Click += new System.EventHandler(this.tsImportData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // tsExit
            // 
            this.tsExit.Name = "tsExit";
            this.tsExit.Size = new System.Drawing.Size(160, 22);
            this.tsExit.Text = "退出";
            this.tsExit.Click += new System.EventHandler(this.tsExit_Click);
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemImportScript,
            this.menuItemImportData,
            this.menuItemExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(44, 21);
            this.menuFile.Text = "文件";
            // 
            // menuItemImportScript
            // 
            this.menuItemImportScript.Name = "menuItemImportScript";
            this.menuItemImportScript.Size = new System.Drawing.Size(124, 22);
            this.menuItemImportScript.Text = "导入脚本";
            // 
            // menuItemImportData
            // 
            this.menuItemImportData.Name = "menuItemImportData";
            this.menuItemImportData.Size = new System.Drawing.Size(124, 22);
            this.menuItemImportData.Text = "导入数据";
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(124, 22);
            this.menuItemExit.Text = "退出";
            // 
            // menuSetting
            // 
            this.menuSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemOption});
            this.menuSetting.Name = "menuSetting";
            this.menuSetting.Size = new System.Drawing.Size(44, 21);
            this.menuSetting.Text = "设置";
            // 
            // menuItemOption
            // 
            this.menuItemOption.Name = "menuItemOption";
            this.menuItemOption.Size = new System.Drawing.Size(100, 22);
            this.menuItemOption.Text = "选项";
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 21);
            this.menuHelp.Text = "帮助";
            // 
            // menuItemAbout
            // 
            this.menuItemAbout.Name = "menuItemAbout";
            this.menuItemAbout.Size = new System.Drawing.Size(100, 22);
            this.menuItemAbout.Text = "关于";
            // 
            // timer
            // 
            this.timer.Interval = 300;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCancelButton,
            this.tsExesqlProgress,
            this.tsInfo});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 460);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(697, 0);
            this.statusStrip1.TabIndex = 11;
            // 
            // tsCancelButton
            // 
            this.tsCancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsCancelButton.Image = ((System.Drawing.Image)(resources.GetObject("tsCancelButton.Image")));
            this.tsCancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsCancelButton.Name = "tsCancelButton";
            this.tsCancelButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tsCancelButton.Size = new System.Drawing.Size(48, 21);
            this.tsCancelButton.Text = "取消";
            this.tsCancelButton.Visible = false;
            this.tsCancelButton.ButtonClick += new System.EventHandler(this.tsCancelButton_ButtonClick);
            // 
            // tsExesqlProgress
            // 
            this.tsExesqlProgress.Name = "tsExesqlProgress";
            this.tsExesqlProgress.Size = new System.Drawing.Size(200, 17);
            this.tsExesqlProgress.Visible = false;
            // 
            // tsInfo
            // 
            this.tsInfo.Name = "tsInfo";
            this.tsInfo.Size = new System.Drawing.Size(103, 17);
            this.tsInfo.Text = "正在执行SQL语句";
            this.tsInfo.Visible = false;
            // 
            // uscExecuteQuery
            // 
            this.uscExecuteQuery.CommandResultText = "";
            this.uscExecuteQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscExecuteQuery.EnableTip = true;
            this.uscExecuteQuery.Location = new System.Drawing.Point(206, 280);
            this.uscExecuteQuery.Name = "uscExecuteQuery";
            this.uscExecuteQuery.Padding = new System.Windows.Forms.Padding(10);
            this.uscExecuteQuery.ShowTip = true;
            this.uscExecuteQuery.Size = new System.Drawing.Size(491, 180);
            this.uscExecuteQuery.SQLText = "";
            this.uscExecuteQuery.SqlTextReadOnly = false;
            this.uscExecuteQuery.Tab = DistDBMS.UserInterface.Controls.UscExecuteQuery.ResultTab.Console;
            this.uscExecuteQuery.TabIndex = 6;
            this.uscExecuteQuery.OnExecuteSQL += new System.EventHandler(this.uscExecuteQuery_OnExecuteSQL);
            // 
            // uscSiteViewer
            // 
            this.uscSiteViewer.Dock = System.Windows.Forms.DockStyle.Top;
            this.uscSiteViewer.Location = new System.Drawing.Point(206, 170);
            this.uscSiteViewer.Name = "uscSiteViewer";
            this.uscSiteViewer.Padding = new System.Windows.Forms.Padding(10);
            this.uscSiteViewer.Size = new System.Drawing.Size(491, 110);
            this.uscSiteViewer.TabIndex = 9;
            // 
            // uscFragmentViewer
            // 
            this.uscFragmentViewer.Dock = System.Windows.Forms.DockStyle.Top;
            this.uscFragmentViewer.Location = new System.Drawing.Point(206, 25);
            this.uscFragmentViewer.Name = "uscFragmentViewer";
            this.uscFragmentViewer.Size = new System.Drawing.Size(491, 145);
            this.uscFragmentViewer.TabIndex = 10;
            // 
            // FrmApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 460);
            this.Controls.Add(this.uscExecuteQuery);
            this.Controls.Add(this.uscSiteViewer);
            this.Controls.Add(this.uscFragmentViewer);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "FrmApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "分布式数据库实验系统";
            this.Shown += new System.EventHandler(this.FrmApp_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmApp_FormClosed);
            this.pnlControl.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvwMenu;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnlControl;
        private DistDBMS.UserInterface.Controls.UscExecuteQuery uscExecuteQuery;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuSetting;
        private System.Windows.Forms.ToolStripMenuItem menuItemOption;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
        private System.Windows.Forms.ToolStripMenuItem menuItemImportScript;
        private System.Windows.Forms.ToolStripMenuItem menuItemImportData;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsExit;
        private System.Windows.Forms.Timer timer;
        private DistDBMS.UserInterface.Controls.UscSiteViewer uscSiteViewer;
        private DistDBMS.UserInterface.Controls.UscFragmentViewer uscFragmentViewer;
        private System.Windows.Forms.ToolStripMenuItem tsWizzard;
        private System.Windows.Forms.ToolStripMenuItem tsImportScript;
        private System.Windows.Forms.ToolStripMenuItem tsImportData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsInfo;
        private System.Windows.Forms.ToolStripProgressBar tsExesqlProgress;
        private System.Windows.Forms.ToolStripSplitButton tsCancelButton;
        
    }
}

