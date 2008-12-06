using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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

            description.Add(Stage.ClearNetwork, "清空所有网络平台");
            description.Add(Stage.InitLocalSite, "执行初始化LocalSite脚本");
            description.Add(Stage.InitControlSite, "执行初始化ControlSite脚本");
            description.Add(Stage.InitDb, "初始化数据库");
            description.Add(Stage.InitData, "导入数据");

            current = (Stage)0;
            type = Type.Init;

            UpdateDescription();
        }

        public void ShowDialog(Type type)
        {
            this.type = type;
            this.ShowDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                switch (current)
                {
                    case Stage.ClearNetwork:
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
                            
                            break;
                        }
                    case Stage.InitData:
                        {
                            
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
