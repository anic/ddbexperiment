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
using System.Threading;

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
            if (resultPacket!=null && resultPacket is ServerClientTextObjectPacket
                && (resultPacket as ServerClientTextObjectPacket).Object is ExecutionResult)
            {
                ExecutionResult result = (resultPacket as ServerClientTextObjectPacket).Object as ExecutionResult;
                if ((resultPacket as ServerClientTextObjectPacket).Text == Common.NetworkCommand.RESULT_OK
                    && result.Type == ExecutionResult.ResultType.Data)
                { 
                    //恢复数据
                    int size = resultPacket.ReadInt();
                    result.Data = new Table(size);
                    for (int i = 0; i < size; ++i)
                        result.Data.Tuples.Add(Tuple.FromLineString(resultPacket.ReadString()));

                    result.Data.Schema = result.OptimizedQueryTree.ResultSchema.Clone() as TableSchema;
                }
                return result;
            }
            return null;
        }

        void ExecuteSQLWithOutput()
        {
            try
            {
                DateTime start = DateTime.Now;
                ExecutionResult exResult = ExecuteSQL(uscExecuteQuery.SQLText);
                DateTime end = DateTime.Now;
                TimeSpan span = end - start;
                exResult.Description += "Executes " + span.TotalMilliseconds + "ms";

                uscExecuteQuery.AddCommandResult(exResult.Description);
                uscExecuteQuery.SetResultTable(exResult.Data);
                uscExecuteQuery.SetOptQueryTree(exResult.OptimizedQueryTree);
                uscExecuteQuery.SetRawQueryTree(exResult.RawQueryTree);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Break();
                LogWriter writer = new LogWriter();
                writer.WriteLog(uscExecuteQuery.SQLText + "\r\n" + ex.StackTrace);
                MessageBox.Show("执行出现异常，并已经记录到error.log中", "出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            FinishProgress();
        }

        void uscExecuteQuery_OnExecuteSQL(object sender, EventArgs e)
        {
            ExecuteSQLWithOutput();
            //StartThread();
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

            //uscExecuteQuery.SQLText = "select Product.name,Product.stocks,Producer.name from Product,Producer where Product.producer_id=Producer.id and Producer.location='BJ' and Product.stocks > 4000";
            uscExecuteQuery.SQLText = "select Customer.id,Customer.name,Product.name,Purchase.number from Customer,Product,Purchase where Customer.id=Purchase.customer_id and Product.id=Purchase.product_id and Customer.rank = 1 and Product.stocks > 2000";

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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (tExecute != null && tExecute.IsAlive)
            {
                this.tsExesqlProgress.Value = (this.tsExesqlProgress.Value + this.tsExesqlProgress.Maximum / 10) % this.tsExesqlProgress.Maximum;
            }
            else
                timer.Stop();
        }

        private void MultiThreadsRunSQL()
        {
            ExecutionResult exResult = ExecuteSQL(uscExecuteQuery.SQLText);
        }

        Thread tExecute;
        private void StartThread()
        {
            tExecute = new Thread(new ThreadStart(ExecuteSQLWithOutput));
            tExecute.Start();
            ShowProgress();
        }

        private void FinishProgress()
        {
            timer.Stop();
            this.tsCancelButton.Visible = false;
            this.tsExesqlProgress.Visible = false;
            this.tsExesqlProgress.Value = 0;
            this.tsInfo.Visible = false;
        }

        private void ShowProgress()
        {
            this.tsCancelButton.Visible = true;
            this.tsExesqlProgress.Visible = true;
            this.tsInfo.Visible = true;
            timer.Start();
        }

        private void tsCancelButton_ButtonClick(object sender, EventArgs e)
        {
            if (tExecute != null && tExecute.IsAlive)
            {
                try
                {
                    tExecute.Abort();
                }
                catch { }
            }
            FinishProgress();
        }




    }
}
