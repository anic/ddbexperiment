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
        ControlSite.VirtualInterface2 vInterface;
        GlobalDirectory gdd;
        MenuTreeSwitcher switcher;
        public FrmApp()
        {
            InitializeComponent();

            imageList.Images.Add(Resources.img_magnifier);

            switcher = new MenuTreeSwitcher(tvwMenu);
            switcher.SetControl(uscExecuteQuery);
            switcher.SetControl(uscTableSchemaViewer);


            vInterface = new DistDBMS.ControlSite.VirtualInterface2();
            
            //初始化脚本
            string result;
            vInterface.ImportScript("InitScript.txt",out gdd,out result);
            uscExecuteQuery.AddCommandResult(result);

            //本地设置gdd
            uscExecuteQuery.SetGlobalDirectory(gdd);
            switcher.SetGlobalDirectory(gdd);


            //导入数据
            vInterface.ImportData("Data.txt", out result);
            uscExecuteQuery.AddCommandResult(result);

            
            //uscExecuteQuery.SQLText = "select Course.name, Course.credit_hour, Teacher.name from Course, Teacher where Course.teacher_id=Teacher.id and Course.credit_hour>2 and Teacher.title=3 ";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);
            
        }

        void uscExecuteQuery_OnExecuteSQL(object sender, EventArgs e)
        {
            
            Table data;
            string result;
            vInterface.ExecuteSQL(uscExecuteQuery.SQLText, out data, out result);
            uscExecuteQuery.AddCommandResult(result);
        }


        
        
    }
}
