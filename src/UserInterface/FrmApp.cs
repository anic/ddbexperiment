using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.ControlSite;
using System.IO;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.UserInterface
{
    public partial class FrmApp : Form
    {

        GlobalDirectory gdd;
        public FrmApp()
        {
            InitializeComponent();

            //初始化GDD
            GDDCreator gddCreator = new GDDCreator();
            string path = "InitScript.txt";
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);

                while (!sr.EndOfStream)
                {
                    gddCreator.InsertCommand(sr.ReadLine());
                }
                sr.Close();
                gdd = gddCreator.CreateGDD();
                
            }

            txtSqlInput.GDD = gdd;
            txtSqlInput.KeyUp += new KeyEventHandler(txtSqlInput_KeyUp);
        }

        void txtSqlInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
                btnExcute_Click(this, EventArgs.Empty);
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            MessageBox.Show("执行" + txtSqlInput.Text);
        }
    }
}
