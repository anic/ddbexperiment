using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Network;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.Plan;
using DistDBMS.ControlSite.SQLSyntax.Parser;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.ControlSite.SQLSyntax;
using DistDBMS.ControlSite.RelationalAlgebraUtility;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.RelationalAlgebra;
using DistDBMS.Common.Table;
using DistDBMS.Common;
using System.Net.Sockets;
using System.Collections;

namespace DistDBMS.ControlSite
{
    class PackageProcessor
    {
        /// <summary>
        /// 默认超时时间
        /// </summary>
        int DEFALUT_TIMEOUT_MINISEC = 60000;

        /// <summary>
        /// 数据字典
        /// </summary>
        GlobalDirectory gdd;

        /// <summary>
        /// 站点的名称
        /// </summary>
        string name;

        /// <summary>
        /// 网络配置参数
        /// </summary>
        ClusterConfiguration config;

        /// <summary>
        /// 网络配置参数2
        /// </summary>
        NetworkInitiator initiator;

        internal class LocalSiteFailException : Exception
        {
            public enum ExceptionType
            {
                ConnectionFail,
                Timeout
            }
            public string site;
            public ExceptionType type;
            public LocalSiteFailException(string site, ExceptionType type)
            {
                this.site = site;
                this.type = type;
            }
        }

        public PackageProcessor(string name,ClusterConfiguration config,NetworkInitiator initiator)
        {
            this.name = name;
            this.config = config;
            this.initiator = initiator;
        }

        public bool IsReady { get { return gdd != null; } }

        private LocalSiteClient GetLocalSiteClient(ControlSiteServerConnection conn, string localSiteName)
        {
            LocalSiteClient result = conn.GetLocalSiteClient(localSiteName);
            if (result == null)
                throw new LocalSiteFailException(localSiteName, LocalSiteFailException.ExceptionType.ConnectionFail);
            else
                return result;
        }

        public void PackageProcess(ControlSiteServerConnection conn, ServerClientPacket packet)
        {
            try
            {
                Debug.WriteLine("收到数据");

                Guid newSessionId = Guid.NewGuid();
                //packet
                if (packet is ServerClientPacket)
                {
                    if (packet is ServerClientTextObjectPacket)
                    {
                        if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommand.GDDSCRIPT)
                        {
                            
                            int size = packet.ReadInt();
                            string[] gddScript = new string[size];
                            for (int i = 0; i < size; ++i)
                                gddScript[i] = packet.ReadString();
                            
                            if (ImportScript(gddScript))
                            {
                                ExecutionPackage package = new ExecutionPackage();
                                package.ID = 1;
                                package.Type = ExecutionPackage.PackageType.Gdd;
                                package.Object = gdd;

                                //初始化每个二级接口
                                foreach (Site site in gdd.Sites)
                                {
                                    GetLocalSiteClient(conn, site.Name).SendStepTextObjectPacket(newSessionId,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Common.NetworkCommand.PLAN, package);
                                }

                                foreach (Site site in gdd.Sites)
                                {
                                    NetworkPacket returnPacket = GetLocalSiteClient(conn, site.Name).Packets.WaitAndRead(DEFALUT_TIMEOUT_MINISEC);
                                    if (returnPacket == null)
                                        throw new LocalSiteFailException(site.Name, LocalSiteFailException.ExceptionType.Timeout);

                                    //TODO: 如何read?
                                }
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, gdd);
                            }
                            else
                            {
                                conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_ERROR);
                            }
                        }
                        #region 导入数据
                        else if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommand.DATASCRIPT)
                        {
                            //string[] dataScript = (packet as ServerClientTextObjectPacket).Object as string[];
                            Common.Debug.WriteLine("收到数据");
                            int size = packet.ReadInt();
                            string[] dataScript = new string[size];
                            for (int i = 0; i < size; ++i)
                                dataScript[i] = packet.ReadString();

                            Common.Debug.WriteLine("转换完成");

                            DataImporter importer = new DataImporter(gdd);
                            importer.ImportFromText(dataScript);

                            ImportPlanCreator importPlanCreator = new ImportPlanCreator(gdd);
                            List<ExecutionPlan> plans = importPlanCreator.CreatePlans(importer);

                            Common.Debug.WriteLine("生成计划");
                            List<Table> tables = new List<Table>();
                            foreach (ExecutionPlan p in plans)
                            {
                                tables.Clear();
                                //设置不同的站点
                                ExecutionPackage package = new ExecutionPackage();
                                package.Type = ExecutionPackage.PackageType.PlanData; //plandata
                                package.Object = p;
                                package.ID = 0;
                                //同步执行
                                foreach (ExecutionStep step in p.Steps)
                                {
                                    Table oldTable = step.Table;
                                    tables.Add(oldTable);
                                    step.Table = new Table();
                                    step.Table.Schema = oldTable.Schema; 
                                }

                                NetworkPacket packet2 = GetLocalSiteClient(conn, p.ExecutionSite.Name).EncapsulateStepTextObjectPacket(newSessionId,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Common.NetworkCommand.PLAN, package);

                                StringBuilder sb = new StringBuilder(10*1024*1024);
                                packet2.WriteInt(tables.Count);
                                foreach (Table t in tables)
                                {
                                    packet2.WriteInt(t.Tuples.Count);

                                    foreach (Tuple tuple in t.Tuples)
                                        packet2.WriteString(tuple.GenerateLineString(sb));
                                }
                                GetLocalSiteClient(conn, p.ExecutionSite.Name).SendPacket(packet2);

                            }

                            

                            foreach (ExecutionPlan p in plans)
                            {
                                NetworkPacket returnPacket = GetLocalSiteClient(conn, p.ExecutionSite.Name).Packets.WaitAndRead(DEFALUT_TIMEOUT_MINISEC);
                                if (returnPacket == null)
                                    throw new LocalSiteFailException(p.ExecutionSite.Name, LocalSiteFailException.ExceptionType.Timeout);
                                //TODO:导入期间出错

                            }
                            Common.Debug.WriteLine("执行完成");
                            string result = "Data imported successful";
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);
                        }
                        #endregion
                        #region 执行SQL
                        else if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommand.EXESQL)
                        {
                            ExecutionResult result = new ExecutionResult();
                            //没有数据字典，不查任何东西
                            if (gdd == null)
                            {
                                result.Description = "Gdd does not exist";
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                                return;
                            }

                            string sql = (packet as ServerClientTextObjectPacket).Object as string;
                            ParserSwitcher ps = new ParserSwitcher();
                            bool bParse = ps.Parse(sql.Trim(), gdd);

                            if (!bParse)
                            {

                                result.Description = "Parse sql error. \nInfo:" + ps.LastError;
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                                return;
                            }
                            
                            #region 查询
                            if (ps.LastResult is Selection)
                            {
                                Selection s3 = ps.LastResult as Selection;

                                GlobalConsitencyFiller filler = new GlobalConsitencyFiller();
                                //这一步很重要，通过gdd来填写selection的完整信息
                                bool checkIntergrity = filler.FillSelection(gdd, s3);
                                if (!checkIntergrity)
                                {
                                    result.Description = "Syntax error.";
                                    conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                                    return;
                                }
                                //可以从这里开始测试转换

                                SQL2RelationalAlgebraInterface converter = new NaiveSQL2RelationalAlgebraConverter();
                                converter.SetQueryCalculus(s3);
                                Relation relationalgebra = converter.SQL2RelationalAlgebra(gdd, true);


                                //原始的查询树
                                SQL2RelationalAlgebraInterface converter2 = new NaiveSQL2RelationalAlgebraConverter();
                                converter2.SetQueryCalculus(s3);
                                result.RawQueryTree = converter.SQL2RelationalAlgebra(gdd, false);

                                //TODO 这里作为测试，临时修改，填写ResultName
                                TempModifier tempModifier = new TempModifier(gdd);
                                tempModifier.Modify(relationalgebra);

                                //测试代码
                                string output = (new RelationDebugger()).GetDebugString(relationalgebra);
                                Common.Debug.WriteLine(output);

                                QueryPlanCreator creator = new QueryPlanCreator(gdd);

                                int id = 0;
                                ExecutionRelation exR = new ExecutionRelation(relationalgebra, ref id, -1);
                                tempModifier.CheckLastSchema(exR, s3.Fields);
                                
                                ExecutionRelation root = (exR.Parent == null) ? exR : exR.Parent;
                                ExecutionPlan gPlan = creator.CreateGlobalPlan(root, 0);

                                output = (new RelationDebugger()).GetDebugString(root);
                                DistDBMS.Common.Debug.WriteLine(output);
                                result.OptimizedQueryTree = root;

                                List<ExecutionPlan> plans = creator.SplitPlan(gPlan);
                                creator.FillSite(root, plans);

                                foreach (ExecutionPlan p in plans)
                                {
                                    Common.Debug.WriteLine(p.ToString());

                                    ExecutionPackage package = new ExecutionPackage();
                                    package.ID = 0;
                                    package.Object = p;
                                    package.Type = ExecutionPackage.PackageType.Plan;

                                    GetLocalSiteClient(conn, p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Common.NetworkCommand.PLAN, package);
                                }

                                NetworkPacket lastPackage = null; //表示数据的那个packet

                                foreach (ExecutionPlan p in plans)
                                {
                                    NetworkPacket returnPackage = GetLocalSiteClient(conn, p.ExecutionSite.Name).Packets.WaitAndRead(DEFALUT_TIMEOUT_MINISEC);
                                    if (returnPackage == null)
                                        throw new LocalSiteFailException(p.ExecutionSite.Name, LocalSiteFailException.ExceptionType.Timeout);

                                    DistDBMS.Common.Debug.WriteLine(name + ": Package from " + p.ExecutionSite.Name);
                                    if (returnPackage is ServerClientTextObjectPacket
                                        && (returnPackage as ServerClientTextObjectPacket).Object != null)
                                    {
                                        if ((returnPackage as ServerClientTextObjectPacket).Object is ExecutionPackage)
                                        {
                                            lastPackage = returnPackage;
                                        }
                                    }
                                }

                                ServerClientPacket okPacket;
                                DistDBMS.Common.Debug.WriteLine("ALL plans done!");
                                if (lastPackage != null)
                                {
                                    int size = lastPackage.ReadInt();
                                    result.Description = "Command executed successfully.\r\n";
                                    result.Description += size + " tuples selected\r\n";
                                    result.Type = ExecutionResult.ResultType.Data;
                                    okPacket = conn.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result, 10 * 1024 * 1024);
                                    okPacket.WriteInt(size);
                                    okPacket.CopyFrom(lastPackage, lastPackage.Pos, lastPackage.Size - lastPackage.Pos);
                                }
                                else if (plans.Count == 0)//不需要执行，所以没有结果
                                {
                                    result.Description = "Command executed successfully.\r\n";
                                    result.Description += "0 tuples selected\r\n";
                                    result.Type = ExecutionResult.ResultType.Data;
                                    okPacket = conn.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);
                                    okPacket.WriteInt(0);
                                }
                                else
                                    okPacket = conn.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);

                                conn.SendPacket(okPacket);

                            }
                            #endregion
                            #region 插入
                            else if (ps.LastResult is Insertion)
                            {
                                Insertion ins = ps.LastResult as Insertion;

                                DataImporter importer = new DataImporter(gdd);
                                importer.ImportFromSql(ins);

                                ImportPlanCreator importPlanCreator = new ImportPlanCreator(gdd);
                                List<ExecutionPlan> plans = importPlanCreator.CreatePlans(importer);

                                result.Description = "The involved site: ";
                                foreach (ExecutionPlan p in plans)
                                {
                                    result.Description += p.ExecutionSite.Name + " ";
                                    //设置不同的站点
                                    ExecutionPackage package = new ExecutionPackage();
                                    package.Type = ExecutionPackage.PackageType.Plan;
                                    package.Object = p;
                                    package.ID = 0;
                                    //同步执行
                                    GetLocalSiteClient(conn, p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Common.NetworkCommand.PLAN, package);
                                }
                                result.Description += "\r\n";

                                foreach (ExecutionPlan p in plans)
                                {

                                    NetworkPacket returnPackage = GetLocalSiteClient(conn, p.ExecutionSite.Name).Packets.WaitAndRead(DEFALUT_TIMEOUT_MINISEC);
                                    if (returnPackage == null)
                                        throw new LocalSiteFailException(p.ExecutionSite.Name, LocalSiteFailException.ExceptionType.Timeout);

                                    DistDBMS.Common.Debug.WriteLine(name + ": Package from " + p.ExecutionSite.Name);

                                    //如果是文字包，则一定是成功的
                                    if (returnPackage is ServerClientTextObjectPacket)
                                    {
                                        ServerClientTextObjectPacket scp = (returnPackage as ServerClientTextObjectPacket);
                                        if (scp.Text == Common.NetworkCommand.RESULT_ERROR)
                                        {
                                            result.Description = "Data insert fail:\r\n" + scp.Object as string;
                                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                                            return;
                                        }
                                    }

                                }
                                result.Description += "Data insert successful";
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);

                            }
                            #endregion
                            else if (ps.LastResult is Deletion)
                            {
                                Deletion del = ps.LastResult as Deletion;
                                DataDeletor deletor = new DataDeletor(gdd);
                                List<ExecutionPlan> plans = deletor.DeleteFromSql(del);

                                foreach (ExecutionPlan p in plans)
                                {
                                    result.Description += p.ExecutionSite.Name + " ";
                                    //设置不同的站点
                                    ExecutionPackage package = new ExecutionPackage();
                                    package.Type = ExecutionPackage.PackageType.Plan;
                                    package.Object = p;
                                    package.ID = 0;
                                    //同步执行
                                    GetLocalSiteClient(conn, p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Common.NetworkCommand.PLAN, package);
                                }
                                result.Description += "\r\n";

                                foreach (ExecutionPlan p in plans)
                                {

                                    NetworkPacket returnPackage = GetLocalSiteClient(conn, p.ExecutionSite.Name).Packets.WaitAndRead(DEFALUT_TIMEOUT_MINISEC);
                                    if (returnPackage == null)
                                        throw new LocalSiteFailException(p.ExecutionSite.Name, LocalSiteFailException.ExceptionType.Timeout);

                                    DistDBMS.Common.Debug.WriteLine(name + ": Package from " + p.ExecutionSite.Name);

                                    //如果是文字包，则一定是成功的
                                    if (returnPackage is ServerClientTextObjectPacket)
                                    {
                                        ServerClientTextObjectPacket scp = (returnPackage as ServerClientTextObjectPacket);
                                        if (scp.Text == Common.NetworkCommand.RESULT_ERROR)
                                        {
                                            result.Description = "Data insert fail:\r\n" + scp.Object as string;
                                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                                            return;
                                        }
                                    }

                                }
                                result.Description += "Data delete successful";
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);
                            }
                            return;
                        }
                        #endregion

                    }
                }
            }
            catch(Exception e){
                ExecutionResult result = new ExecutionResult();
                if (e is LocalSiteFailException)
                {
                    if ((e as LocalSiteFailException).type == LocalSiteFailException.ExceptionType.ConnectionFail)
                        result.Description = "Site " + (e as LocalSiteFailException).site + " failed.";
                    else
                        result.Description = "Site " + (e as LocalSiteFailException).site + " wait timeout.\r\n";
                }
                else if (e is System.Net.Sockets.SocketException)
                {
                    
                    SocketException ex = (e as System.Net.Sockets.SocketException);
                
                    //查询哪一个LocalSite站点出错
                    foreach (string localSite in initiator.LocalSiteNames)
                    {

                        string host = config.Hosts[localSite]["Host"] as string;
                        int port = (int)config.Hosts[localSite]["Port"];
                        string addr = host + ":" + port.ToString();
                        if (ex.Message.IndexOf(addr) != -1)
                        {
                            result.Description = "Site " + localSite + " fail\r\n";
                            break;
                        }
                    }
                }
                else
                    result.Description = e.Message + "\r\n";
                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                Common.Debug.WriteLine(result.Description);
            }


        }

        private bool ImportScript(string[] file)
        {
            //初始化GDD
            GDDCreator gddCreator = new GDDCreator();
            foreach (string line in file)
                gddCreator.InsertCommand(line);

            gdd = gddCreator.CreateGDD();
            return gdd != null;
        }


    }
}
