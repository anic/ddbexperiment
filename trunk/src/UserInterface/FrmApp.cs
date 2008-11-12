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
using DistDBMS.UserInterface.Controls;
using DistDBMS.UserInterface.Properties;
using DistDBMS.Common.Table;
using DistDBMS.UserInterface.Handler;

namespace DistDBMS.UserInterface
{
    public partial class FrmApp : Form
    {

        GlobalDirectory gdd;
        MenuTreeSwitcher switcher;
        public FrmApp()
        {
            InitializeComponent();

            imageList.Images.Add(Resources.img_magnifier);

            switcher = new MenuTreeSwitcher(tvwMenu);
            switcher.SetControl(uscExecuteQuery);
            switcher.SetControl(uscTableSchemaViewer);


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

            uscExecuteQuery.SetGlobalDirectory(gdd);
            switcher.SetGlobalDirectory(gdd);
        }


        
        
    }
}
