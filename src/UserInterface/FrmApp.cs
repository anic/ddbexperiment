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

            uscSqlInput1.GDD = gdd;

        }
    }
}
