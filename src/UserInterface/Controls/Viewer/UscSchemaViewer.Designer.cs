﻿namespace DistDBMS.UserInterface.Controls
{
    partial class UscSchemaViewer
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
            this.lblCondition = new System.Windows.Forms.Label();
            this.lblSite = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvwTable
            // 
            this.lvwTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTable.FullRowSelect = true;
            this.lvwTable.Location = new System.Drawing.Point(10, 71);
            this.lvwTable.Name = "lvwTable";
            this.lvwTable.Size = new System.Drawing.Size(475, 236);
            this.lvwTable.TabIndex = 0;
            this.lvwTable.UseCompatibleStateImageBehavior = false;
            this.lvwTable.View = System.Windows.Forms.View.Details;
            // 
            // lblTableName
            // 
            this.lblTableName.AutoSize = true;
            this.lblTableName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTableName.Font = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTableName.Location = new System.Drawing.Point(10, 10);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(82, 21);
            this.lblTableName.TabIndex = 1;
            this.lblTableName.Text = "label1";
            // 
            // lblCondition
            // 
            this.lblCondition.AutoSize = true;
            this.lblCondition.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCondition.Location = new System.Drawing.Point(10, 31);
            this.lblCondition.Name = "lblCondition";
            this.lblCondition.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.lblCondition.Size = new System.Drawing.Size(41, 20);
            this.lblCondition.TabIndex = 2;
            this.lblCondition.Text = "label1";
            // 
            // lblSite
            // 
            this.lblSite.AutoSize = true;
            this.lblSite.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSite.Location = new System.Drawing.Point(10, 51);
            this.lblSite.Name = "lblSite";
            this.lblSite.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.lblSite.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblSite.Size = new System.Drawing.Size(41, 20);
            this.lblSite.TabIndex = 3;
            this.lblSite.Text = "label1";
            // 
            // UscSchemaViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwTable);
            this.Controls.Add(this.lblSite);
            this.Controls.Add(this.lblCondition);
            this.Controls.Add(this.lblTableName);
            this.Name = "UscSchemaViewer";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(495, 317);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwTable;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.Label lblCondition;
        private System.Windows.Forms.Label lblSite;

    }
}
