namespace DistDBMS.UserInterface.Controls
{
    partial class UscTableViewer
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
            this.lvwTable = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvwTable
            // 
            this.lvwTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTable.FullRowSelect = true;
            this.lvwTable.Location = new System.Drawing.Point(0, 0);
            this.lvwTable.MultiSelect = false;
            this.lvwTable.Name = "lvwTable";
            this.lvwTable.Size = new System.Drawing.Size(371, 274);
            this.lvwTable.TabIndex = 0;
            this.lvwTable.UseCompatibleStateImageBehavior = false;
            this.lvwTable.View = System.Windows.Forms.View.Details;
            // 
            // UscTableViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwTable);
            this.Name = "UscTableViewer";
            this.Size = new System.Drawing.Size(371, 274);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwTable;
    }
}
