namespace DistDBMS.UserInterface.SqlInput
{
    partial class FrmTip
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
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.lsbTip = new DistDBMS.UserInterface.SqlInput.GListBox.GListBox();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(17, 17);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lsbTip
            // 
            this.lsbTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbTip.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lsbTip.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lsbTip.FormattingEnabled = true;
            this.lsbTip.ImageList = this.imageList;
            this.lsbTip.ItemHeight = 18;
            this.lsbTip.LevelWidth = 10;
            this.lsbTip.Location = new System.Drawing.Point(0, 0);
            this.lsbTip.Name = "lsbTip";
            this.lsbTip.Size = new System.Drawing.Size(198, 238);
            this.lsbTip.TabIndex = 0;
            // 
            // FrmTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 245);
            this.Controls.Add(this.lsbTip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTip";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SQL Tip";
            this.ResumeLayout(false);

        }

        #endregion

        private DistDBMS.UserInterface.SqlInput.GListBox.GListBox lsbTip;
        private System.Windows.Forms.ImageList imageList;




    }
}