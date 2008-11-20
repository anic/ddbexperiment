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
            this.tvwRelation = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvwRelation
            // 
            this.tvwRelation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwRelation.Location = new System.Drawing.Point(0, 0);
            this.tvwRelation.Name = "tvwRelation";
            this.tvwRelation.Size = new System.Drawing.Size(232, 253);
            this.tvwRelation.TabIndex = 0;
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
    }
}
