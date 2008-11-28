namespace DistDBMS.UserInterface.Controls
{
    partial class UscSiteViewer
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
            this.lvwSite = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.lblSitename = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvwSite
            // 
            this.lvwSite.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwSite.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvwSite.Location = new System.Drawing.Point(10, 34);
            this.lvwSite.Name = "lvwSite";
            this.lvwSite.Size = new System.Drawing.Size(383, 87);
            this.lvwSite.TabIndex = 0;
            this.lvwSite.UseCompatibleStateImageBehavior = false;
            this.lvwSite.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名称";
            this.columnHeader1.Width = 98;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP";
            this.columnHeader2.Width = 104;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "端口";
            this.columnHeader3.Width = 93;
            // 
            // lblSitename
            // 
            this.lblSitename.AutoSize = true;
            this.lblSitename.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSitename.Font = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSitename.Location = new System.Drawing.Point(10, 10);
            this.lblSitename.Name = "lblSitename";
            this.lblSitename.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.lblSitename.Size = new System.Drawing.Size(82, 24);
            this.lblSitename.TabIndex = 2;
            this.lblSitename.Text = "label1";
            // 
            // UscSiteViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwSite);
            this.Controls.Add(this.lblSitename);
            this.Name = "UscSiteViewer";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(403, 368);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwSite;
        private System.Windows.Forms.Label lblSitename;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
