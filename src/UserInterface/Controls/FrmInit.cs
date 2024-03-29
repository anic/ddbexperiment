﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DistDBMS.Network;
using DistDBMS.UserInterface.Handler;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Dictionary;
using DistDBMS.UserInterface.Properties;

namespace DistDBMS.UserInterface.Controls
{
    public partial class FrmInit : Form
    {
        public enum Type
        { 
            Init,
            Destroy
        }

        enum Stage
        {
            ClearNetwork = 0,
            ClearDb,
            InitLocalSite,
            InitControlSite,
            InitDb,
            InitData,
            Length
        };

        private Dictionary<Stage, string> scripts = new Dictionary<Stage, string>() ;
        private Dictionary<Stage, string> description = new Dictionary<Stage, string>();
        

        private Stage current;
        private Type type;
        GlobalDirectory gdd;

        public FrmInit()
        {
        
            InitializeComponent();

            scripts.Add(Stage.InitLocalSite, "RunLocalSite.bat");
            scripts.Add(Stage.InitControlSite, "RunControlSite.bat");
            scripts.Add(Stage.ClearNetwork, "KillAllSites.bat");
            scripts.Add(Stage.ClearDb, "DeleteDb.bat");

            description.Add(Stage.ClearNetwork, "清空所有网络平台");
            description.Add(Stage.InitLocalSite, "执行初始化LocalSite脚本");
            description.Add(Stage.InitControlSite, "执行初始化ControlSite脚本");
            description.Add(Stage.InitDb, "执行初始化数据库脚本 "+Resources.FILE_DBSCRIPT);
            description.Add(Stage.InitData, "导入数据文件 " + Resources.FILE_DATA);
            description.Add(Stage.ClearDb, "删除当前计算机上的所有数据库");
            
            //TODO:先从初始化开始，以后改成清空平台
            //current = (Stage)0;
            current = Stage.ClearNetwork;

            type = Type.Init;

            UpdateDescription();
        }


        ClusterConfiguration clusterConfig;
        public void ShowDialog(Type type, ClusterConfiguration clusterConfig)
        {
            this.type = type;
            this.clusterConfig = clusterConfig;
            this.ShowDialog();
        }

        public void RunDefaultStage( ClusterConfiguration clusterConfig)
        {
            this.type = Type.Init;
            this.clusterConfig = clusterConfig;
            /*int[] stepList = new int[] { 
                (int)Stage.ClearDb, 
                (int)Stage.InitDb,
                (int)Stage.InitData
            };*/

            int[] stepList = new int[] { 
                (int)Stage.InitDb
            };



            foreach (int step in stepList)
            {
                current = (Stage)step;
                btnOk_Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 全局数据字典
        /// </summary>
        public GlobalDirectory GDD { get { return gdd; } }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (current)
                {
                    case Stage.ClearNetwork:
                    case Stage.ClearDb:
                    case Stage.InitLocalSite:
                    case Stage.InitControlSite:
                        {
                            
                            string cmd = scripts[current];
                            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(@"BatchScript\" + cmd);
                            proc.WaitForExit();
                            break;
                        }
                    case Stage.InitDb:
                        {
                            ControlSiteClient controlSiteClient = new ControlSiteClient();
                            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);

                            string[] gddScript = FileUploader.ReadFileToString(Resources.FILE_DBSCRIPT);
                            NetworkPacket packet = controlSiteClient.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.GDDSCRIPT, 0);
                            packet.EnsureSize(10 * 1024 * 1024);

                            packet.WriteInt(gddScript.Length);
                            int size = gddScript.Length;
                            for (int i = 0; i < size; ++i)
                                packet.WriteString(gddScript[i]);
                            controlSiteClient.SendPacket(packet);

                            //controlSiteClient.SendServerClientTextObjectPacket(Common.NetworkCommand.GDDSCRIPT, gddScript);
                            NetworkPacket returnPacket = controlSiteClient.Packets.WaitAndRead();
                            if (returnPacket is ServerClientTextObjectPacket)
                            {
                                if ((returnPacket as ServerClientTextObjectPacket).Text == Common.NetworkCommand.RESULT_OK)
                                    gdd = (returnPacket as ServerClientTextObjectPacket).Object as GlobalDirectory;
                                else
                                    gdd = null;
                            }
                            
                            break;
                        }
                    case Stage.InitData:
                        {
                            ControlSiteClient controlSiteClient = new ControlSiteClient();
                            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);

                            string[] dataScript = FileUploader.ReadFileToString(Resources.FILE_DATA);

                            NetworkPacket packet = controlSiteClient.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.DATASCRIPT, 0);
                            packet.EnsureSize(10 * 1024 * 1024);

                            packet.WriteInt(dataScript.Length);
                            int size = dataScript.Length;
                            for (int i = 0; i < size; ++i)
                                packet.WriteString(dataScript[i]);
                            controlSiteClient.SendPacket(packet);

                            //controlSiteClient.SendServerClientTextObjectPacket(Common.NetworkCommand.DATASCRIPT, dataScript);
                            controlSiteClient.Packets.WaitAndRead();

                            break;
                        }
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "异常警告");
            }

            NextStage();
        }

        private void NextStage()
        {
            

            if (type == Type.Init)
                current = (Stage)(current + 1);
            else
                current = Stage.Length;

            if (current >= Stage.Length)
            {
                this.Close();
                return;
            }

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            lblStep.Text = ((int)current + 1).ToString();
            lblDescription.Text = "是否" + description[current];
            this.Focus();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            NextStage();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
           for (int i = 0; i < (int)Stage.Length; i++)
                btnOk_Click(this, e);
        
        }

        private void FrmInit_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }

    }
}
