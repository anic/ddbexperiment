﻿using System;
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
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Network;
using DistDBMS.Common.Execution;

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

            vInterface = new DistDBMS.ControlSite.VirtualInterface2();

            FrmInit frmInit = new FrmInit();
            frmInit.ShowDialog(FrmInit.Type.Init);


            NetworkInitiator initiator = new NetworkInitiator();
            ClusterConfiguration clusterConfig = initiator.GetConfiguration("NetworkInitScript.txt");


            ControlSiteClient controlSiteClient = new ControlSiteClient();
            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);

            StreamReader sr = new StreamReader("DbInitScript.txt", System.Text.Encoding.Default);
            List<string> gddScript = new List<string>();
            while (!sr.EndOfStream)
                gddScript.Add(sr.ReadLine());
            sr.Close();


            ServerClientTextObjectPacket package = new ServerClientTextObjectPacket();
            package.Object = gddScript;
            //controlSiteClient.SendPacket(package);

            //ServerClientPacket csPacket = ServerClientPacket.NetworkPacketToServerClientPacket(controlSiteClient.Packets.WaitAndRead());
            //if (csPacket is ServerClientTextObjectPacket)
            //{
            //    Debug.WriteLine((csPacket as ServerClientTextObjectPacket).Object.ToString());
            //}

            //初始化脚本
            string result;
            vInterface.ImportScript("DbInitScript.txt", out gdd, out result);
            uscExecuteQuery.AddCommandResult(result);

            //本地设置gdd
            uscExecuteQuery.SetGlobalDirectory(gdd);
            switcher.SetGlobalDirectory(gdd);


            //导入数据
            vInterface.ImportData("Data.txt", out result);
            uscExecuteQuery.AddCommandResult(result);

            uscExecuteQuery.EnableTip = false;
        }

        public void ExecuteSQL()
        {
            uscExecuteQuery_OnExecuteSQL(this, EventArgs.Empty);
        }

        void uscExecuteQuery_OnExecuteSQL(object sender, EventArgs e)
        {

            Table data;
            string result;
            Relation queryTree;
            try
            {
                vInterface.ExecuteSQL(uscExecuteQuery.SQLText, out data, out result, out queryTree);
                uscExecuteQuery.AddCommandResult(result);
                uscExecuteQuery.SetResultTable(data);
                uscExecuteQuery.SetQueryTree(queryTree);
            }
            catch (Exception ex)
            {
                LogWriter writer = new LogWriter();
                writer.WriteLog(uscExecuteQuery.SQLText + "\r\n" + ex.StackTrace);
                MessageBox.Show("执行出现异常，并已经记录到error.log中", "出错", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }

        }

        private void FrmApp_Shown(object sender, EventArgs e)
        {
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
            FrmInit frmDestroy = new FrmInit();
            frmDestroy.ShowDialog(FrmInit.Type.Destroy);
        }




    }
}
