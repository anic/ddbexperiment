using System;
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
            description.Add(Stage.InitDb, "初始化数据库");
            description.Add(Stage.InitData, "导入数据");
            description.Add(Stage.ClearDb, "删除所有数据库");
            
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

                            string[] gddScript = FileUploader.ReadFileToString("DbInitScript.txt");
                            controlSiteClient.SendServerClientTextObjectPacket(Common.NetworkCommand.GDDSCRIPT, gddScript);
                            controlSiteClient.Packets.WaitAndRead();
                            
                            break;
                        }
                    case Stage.InitData:
                        {
                            ControlSiteClient controlSiteClient = new ControlSiteClient();
                            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);

                            string[] dataScript = FileUploader.ReadFileToString("Data.txt");
                            controlSiteClient.SendServerClientTextObjectPacket(Common.NetworkCommand.DATASCRIPT, dataScript);
                            controlSiteClient.Packets.WaitAndRead();

                            break;
                            break;
                        }
                }

            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

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
