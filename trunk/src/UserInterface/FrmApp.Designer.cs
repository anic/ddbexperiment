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
            this.txtSqlInput = new DistDBMS.UserInterface.SqlInput.SqlTextBox();
            this.btnExcute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSqlInput
            // 
            this.txtSqlInput.BackColor = System.Drawing.Color.White;
            this.txtSqlInput.ForeColor = System.Drawing.Color.Black;
            this.txtSqlInput.GDD = null;
            this.txtSqlInput.Location = new System.Drawing.Point(99, 27);
            this.txtSqlInput.Name = "txtSqlInput";
            this.txtSqlInput.Size = new System.Drawing.Size(404, 210);
            inputStyle1.BackgroundColor = System.Drawing.Color.White;
            inputStyle1.ForeColor = System.Drawing.Color.Black;
            inputStyle1.KeywordColor = System.Drawing.Color.Blue;
            this.txtSqlInput.Style = inputStyle1;
            this.txtSqlInput.TabIndex = 0;
            this.txtSqlInput.Text = " ";
            // 
            // btnExcute
            // 
            this.btnExcute.Location = new System.Drawing.Point(428, 243);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(75, 23);
            this.btnExcute.TabIndex = 1;
            this.btnExcute.Text = "执行";
            this.btnExcute.UseVisualStyleBackColor = true;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // FrmApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 276);
            this.Controls.Add(this.btnExcute);
            this.Controls.Add(this.txtSqlInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DistDBMS.UserInterface.SqlInput.SqlTextBox txtSqlInput;
        private System.Windows.Forms.Button btnExcute;
    }
}

