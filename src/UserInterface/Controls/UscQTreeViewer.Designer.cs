namespace DistDBMS.UserInterface.Controls
{
    partial class UscQTreeViewer
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
            this.components = new System.ComponentModel.Container();
            this.tvwRelation = new System.Windows.Forms.TreeView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // tvwRelation
            // 
            this.tvwRelation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwRelation.ImageIndex = 0;
            this.tvwRelation.ImageList = this.imgList;
            this.tvwRelation.Location = new System.Drawing.Point(0, 0);
            this.tvwRelation.Name = "tvwRelation";
            this.tvwRelation.SelectedImageIndex = 0;
            this.tvwRelation.Size = new System.Drawing.Size(232, 253);
            this.tvwRelation.TabIndex = 0;
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // UscQTreeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvwRelation);
            this.Name = "UscQTreeViewer";
            this.Size = new System.Drawing.Size(232, 253);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvwRelation;
        private System.Windows.Forms.ImageList imgList;
    }
}
