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
            DistDBMS.UserInterface.SqlInput.InputStyle inputStyle1 = new DistDBMS.UserInterface.SqlInput.InputStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmApp));
            this.uscSqlInput1 = new DistDBMS.UserInterface.SqlInput.SqlTextBox();
            this.SuspendLayout();
            // 
            // uscSqlInput1
            // 
            this.uscSqlInput1.BackColor = System.Drawing.Color.White;
            this.uscSqlInput1.ForeColor = System.Drawing.Color.Black;
            this.uscSqlInput1.GDD = null;
            this.uscSqlInput1.Location = new System.Drawing.Point(189, 25);
            this.uscSqlInput1.Name = "uscSqlInput1";
            this.uscSqlInput1.Size = new System.Drawing.Size(314, 100);
            inputStyle1.BackgroundColor = System.Drawing.Color.White;
            inputStyle1.ForeColor = System.Drawing.Color.Black;
            inputStyle1.KeywordColor = System.Drawing.Color.Blue;
            this.uscSqlInput1.Style = inputStyle1;
            this.uscSqlInput1.TabIndex = 0;
            this.uscSqlInput1.Text = " ";
            // 
            // FrmApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 276);
            this.Controls.Add(this.uscSqlInput1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DistDBMS.UserInterface.SqlInput.SqlTextBox uscSqlInput1;
    }
}

