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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("优化树");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("查询", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("站点");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("表");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("分片");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("数据字典", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmApp));
            this.tvwMenu = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportScript = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemImportData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOption = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.uscTableSchemaViewer = new DistDBMS.UserInterface.Controls.UscTableSchemaViewer();
            this.uscExecuteQuery = new DistDBMS.UserInterface.Controls.UscExecuteQuery();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlControl.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvwMenu
            // 
            this.tvwMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwMenu.ItemHeight = 23;
            this.tvwMenu.Location = new System.Drawing.Point(10, 10);
            this.tvwMenu.Name = "tvwMenu";
            treeNode1.Name = "Node2";
            treeNode1.Text = "优化树";
            treeNode2.ImageIndex = -2;
            treeNode2.Name = "Node0";
            treeNode2.Text = "查询";
            treeNode3.Name = "Node3";
            treeNode3.Text = "站点";
            treeNode4.Name = "Node4";
            treeNode4.Text = "表";
            treeNode5.Name = "Node6";
            treeNode5.Text = "分片";
            treeNode6.Name = "Node1";
            treeNode6.Text = "数据字典";
            this.tvwMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode6});
            this.tvwMenu.Size = new System.Drawing.Size(183, 415);
            this.tvwMenu.TabIndex = 2;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(30, 30);
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
            // uscTableSchemaViewer
            // 
            this.uscTableSchemaViewer.Location = new System.Drawing.Point(281, 109);
            this.uscTableSchemaViewer.Name = "uscTableSchemaViewer";
            this.uscTableSchemaViewer.Padding = new System.Windows.Forms.Padding(10);
            this.uscTableSchemaViewer.Size = new System.Drawing.Size(495, 317);
            this.uscTableSchemaViewer.TabIndex = 8;
            // 
            // uscExecuteQuery
            // 
            this.uscExecuteQuery.Location = new System.Drawing.Point(203, 64);
            this.uscExecuteQuery.Name = "uscExecuteQuery";
            this.uscExecuteQuery.Padding = new System.Windows.Forms.Padding(10);
            this.uscExecuteQuery.Size = new System.Drawing.Size(422, 315);
            this.uscExecuteQuery.TabIndex = 6;
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            // 
            // FrmApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 460);
            this.Controls.Add(this.uscTableSchemaViewer);
            this.Controls.Add(this.uscExecuteQuery);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlControl);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "FrmApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "分布式数据库实验系统";
            this.pnlControl.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
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
        private DistDBMS.UserInterface.Controls.UscTableSchemaViewer uscTableSchemaViewer;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        
    }
}

