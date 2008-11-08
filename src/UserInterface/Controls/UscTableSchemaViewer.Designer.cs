namespace DistDBMS.UserInterface.Controls
{
    partial class UscTableSchemaViewer
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
            this.lblTableName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvwTable
            // 
            this.lvwTable.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lvwTable.Location = new System.Drawing.Point(10, 37);
            this.lvwTable.Name = "lvwTable";
            this.lvwTable.Size = new System.Drawing.Size(475, 270);
            this.lvwTable.TabIndex = 0;
            this.lvwTable.UseCompatibleStateImageBehavior = false;
            this.lvwTable.View = System.Windows.Forms.View.Details;
            // 
            // label1
            // 
            this.lblTableName.AutoSize = true;
            this.lblTableName.Font = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTableName.Location = new System.Drawing.Point(6, 13);
            this.lblTableName.Name = "label1";
            this.lblTableName.Size = new System.Drawing.Size(82, 21);
            this.lblTableName.TabIndex = 1;
            this.lblTableName.Text = "label1";
            // 
            // UscTableSchemaViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTableName);
            this.Controls.Add(this.lvwTable);
            this.Name = "UscTableSchemaViewer";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(495, 317);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwTable;
        private System.Windows.Forms.Label lblTableName;

    }
}
