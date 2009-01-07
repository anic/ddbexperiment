using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DistDBMS.Common.Dictionary;
using DistDBMS.UserInterface.Controls;
using DistDBMS.UserInterface.Properties;
using DistDBMS.Common.Table;
using DistDBMS.UserInterface.Handler;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Network;
using DistDBMS.Common.Execution;

namespace DistDBMS.UserInterface
{
    public partial class FrmApp : Form
    {
        GlobalDirectory gdd;
        MenuTreeSwitcher switcher;
        ClusterConfiguration clusterConfig;

        public bool NeedWizzard { get; set; }

        public FrmApp()
        {
            InitializeComponent();

            imageList.Images.Add("magnifier", Resources.img_magnifier);
            imageList.Images.Add("dictionary", Resources.img_dictionary);
            imageList.Images.Add("site", Resources.img_site);
            imageList.Images.Add("table", Resources.img_table);
            imageList.Images.Add("hfragment", Resources.img_hfragment);
            imageList.Images.Add("vfragment", Resources.img_vfragment);

            switcher = new MenuTreeSwitcher(tvwMenu, this);
            switcher.SetControl(uscExecuteQuery);
            switcher.SetControl(uscFragmentViewer);
            switcher.SetControl(uscSiteViewer);

            NeedWizzard = true;

            NetworkInitiator initiator = new NetworkInitiator();
            clusterConfig = initiator.GetConfiguration(Resources.FILE_NETWORKSCRIPT);

            uscExecuteQuery.EnableTip = false;
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public ExecutionResult ExecuteSQL(string sql)
        {

            //TODO:设置选择ControlSite
            ControlSiteClient controlSiteClient = new ControlSiteClient();
            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);
            controlSiteClient.SendServerClientTextObjectPacket(Common.NetworkCommand.EXESQL, sql);

            NetworkPacket resultPacket = controlSiteClient.Packets.WaitAndRead();
            if (resultPacket is ServerClientTextObjectPacket
                && (resultPacket as ServerClientTextObjectPacket).Object is ExecutionResult)
            {
                return (resultPacket as ServerClientTextObjectPacket).Object as ExecutionResult;
            }
            return null;
        }

        void uscExecuteQuery_OnExecuteSQL(object sender, EventArgs e)
        {
            try
            {
                ExecutionResult exResult = ExecuteSQL(uscExecuteQuery.SQLText);
                uscExecuteQuery.AddCommandResult(exResult.Description);
                uscExecuteQuery.SetResultTable(exResult.Data);
                uscExecuteQuery.SetQueryTree(exResult.RawQueryTree);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Break();
                LogWriter writer = new LogWriter();
                writer.WriteLog(uscExecuteQuery.SQLText + "\r\n" + ex.StackTrace);
                MessageBox.Show("执行出现异常，并已经记录到error.log中", "出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RunDefaultWizzard()
        {
            FrmInit frmInit = new FrmInit();
            frmInit.RunDefaultStage(clusterConfig);

            if (frmInit.GDD != null)
            {
                uscExecuteQuery.SetGlobalDirectory(frmInit.GDD);
                switcher.SetGlobalDirectory(frmInit.GDD);
            }


        }

        private void FrmApp_Shown(object sender, EventArgs e)
        {
            FrmInit frmInit = new FrmInit();
            if (NeedWizzard)
                frmInit.ShowDialog(FrmInit.Type.Init, clusterConfig);
            else
                frmInit.RunDefaultStage(clusterConfig);

            if (frmInit.GDD != null)
            {
                uscExecuteQuery.SetGlobalDirectory(frmInit.GDD);
                switcher.SetGlobalDirectory(frmInit.GDD);
            }


            //测试执行sql

            //uscExecuteQuery.SQLText = "select * from Student";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);

            //uscExecuteQuery.SQLText = "select Course.name from Course";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);

            //uscExecuteQuery.SQLText = "select * from Course where credit_hour>2 and location='CB‐6'";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);

            //uscExecuteQuery.SQLText = "select course_id, mark from Exam";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);

            //uscExecuteQuery.SQLText = "select Course.name, Course.credit_hour, Teacher.name from Course, Teacher where Course.teacher_id=Teacher.id and Course.credit_hour>2 and Teacher.title=3";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);


            //uscExecuteQuery.SQLText = "select Student.name, Exam.mark from Student, Exam where Student.id=Exam.student_id";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);


            //uscExecuteQuery.SQLText = "select Student.id, Student.name, Exam.mark, Course.name from Student, Exam, Course where Student.id=Exam.student_id and Exam.course_id=Course.id and Student.age>26 and Course.location<>'CB‐6'";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);

            //uscExecuteQuery.SQLText = "select Student.name, Teacher.name from Student, Teacher,Course where Student.id = Teacher.id and Course.teacher_id = Teacher.id";
            //uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);


            uscExecuteQuery.EnableTip = true;

        }

        private void FrmApp_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (NeedWizzard)
            {
                FrmInit frmDestroy = new FrmInit();
                frmDestroy.ShowDialog(FrmInit.Type.Destroy, null);
            }
        }

        private void tsWizzard_Click(object sender, EventArgs e)
        {
            FrmInit frmInit = new FrmInit();
            frmInit.ShowDialog(FrmInit.Type.Init, clusterConfig);

            if (frmInit.GDD != null)
            {
                uscExecuteQuery.SetGlobalDirectory(frmInit.GDD);
                switcher.SetGlobalDirectory(frmInit.GDD);
            }

        }

        private void tsImportScript_Click(object sender, EventArgs e)
        {

        }

        private void tsImportData_Click(object sender, EventArgs e)
        {

        }

        private void tsExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }




    }
}
