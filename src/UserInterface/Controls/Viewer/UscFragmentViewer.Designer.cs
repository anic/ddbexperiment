namespace DistDBMS.UserInterface.Controls
{
    partial class UscFragmentViewer
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
            this.pnlLayout = new System.Windows.Forms.TableLayoutPanel();
            this.uscSchemaViewer = new DistDBMS.UserInterface.Controls.UscSchemaViewer();
            this.uscFragmentShower = new DistDBMS.UserInterface.Controls.UscFragmentShower();
            this.pnlLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLayout
            // 
            this.pnlLayout.ColumnCount = 2;
            this.pnlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.pnlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.pnlLayout.Controls.Add(this.uscSchemaViewer, 0, 0);
            this.pnlLayout.Controls.Add(this.uscFragmentShower, 1, 0);
            this.pnlLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLayout.Location = new System.Drawing.Point(0, 0);
            this.pnlLayout.Name = "pnlLayout";
            this.pnlLayout.RowCount = 1;
            this.pnlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlLayout.Size = new System.Drawing.Size(588, 210);
            this.pnlLayout.TabIndex = 0;
            // 
            // uscSchemaViewer
            // 
            this.uscSchemaViewer.BackColor = System.Drawing.SystemColors.Control;
            this.uscSchemaViewer.Condition = "label1";
            this.uscSchemaViewer.CurrentSite = null;
            this.uscSchemaViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscSchemaViewer.Location = new System.Drawing.Point(3, 3);
            this.uscSchemaViewer.Name = "uscSchemaViewer";
            this.uscSchemaViewer.Padding = new System.Windows.Forms.Padding(10);
            this.uscSchemaViewer.Size = new System.Drawing.Size(405, 204);
            this.uscSchemaViewer.TabIndex = 0;
            // 
            // uscFragmentShower
            // 
            this.uscFragmentShower.BackColor = System.Drawing.SystemColors.Control;
            this.uscFragmentShower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uscFragmentShower.Location = new System.Drawing.Point(414, 3);
            this.uscFragmentShower.Name = "uscFragmentShower";
            this.uscFragmentShower.Size = new System.Drawing.Size(171, 204);
            this.uscFragmentShower.TabIndex = 1;
            // 
            // UscFragmentViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlLayout);
            this.Name = "UscFragmentViewer";
            this.Size = new System.Drawing.Size(588, 210);
            this.pnlLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlLayout;
        private UscSchemaViewer uscSchemaViewer;
        private UscFragmentShower uscFragmentShower;
    }
}
