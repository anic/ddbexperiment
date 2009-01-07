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

namespace DistDBMS.ControlSite
{
    class PackageProcessor
    {
        GlobalDirectory gdd;
        string name;

        public PackageProcessor(string name)
        {
            this.name = name;
        }
        
        public bool IsReady { get { return gdd != null; } }


        public void PackageProcess(ControlSiteServerConnection conn, ServerClientPacket packet)
        {
            Guid newSessionId = Guid.NewGuid();
            //packet
            if (packet is ServerClientPacket) 
            {
                if (packet is ServerClientTextObjectPacket)
                {
                    if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommand.GDDSCRIPT)
                    {
                        string[] gddScript = (packet as ServerClientTextObjectPacket).Object as string[];
                        if (ImportScript(gddScript))
                        {
                            ExecutionPackage package = new ExecutionPackage();
                            package.ID = 1;
                            package.Type = ExecutionPackage.PackageType.Gdd;
                            package.Object = gdd;

                            //初始化每个二级接口
                            foreach (Site site in gdd.Sites)
                            {
                                conn.GetLocalSiteClient(site.Name).SendStepTextObjectPacket(newSessionId,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Common.NetworkCommand.PLAN, package);
                            }

                            foreach (Site site in gdd.Sites)
                            {
                                NetworkPacket returnPacket = conn.GetLocalSiteClient(site.Name).Packets.WaitAndRead();
                                
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
                        string[] dataScript = (packet as ServerClientTextObjectPacket).Object as string[];
                        DataImporter importer = new DataImporter(gdd);
                        importer.ImportFromText(dataScript);

                        ImportPlanCreator importPlanCreator = new ImportPlanCreator(gdd);
                        List<ExecutionPlan> plans = importPlanCreator.CreatePlans(importer);

                        foreach (ExecutionPlan p in plans)
                        {
                            //设置不同的站点
                            ExecutionPackage package = new ExecutionPackage();
                            package.Type = ExecutionPackage.PackageType.Plan;
                            package.Object = p;
                            package.ID = 0;
                            //同步执行
                            //(virInterfaces[p.ExecutionSite.Name] as VirtualInterface).ReceiveExecutionPackage(package);
                            conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Common.NetworkCommand.PLAN, package);
                        }

                        foreach (ExecutionPlan p in plans)
                        {
                            conn.GetLocalSiteClient(p.ExecutionSite.Name).Packets.WaitAndRead();
                            //TODO:read

                        }

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
                            //TO 刘璋：可以从这里开始测试转换

                            SQL2RelationalAlgebraInterface converter = new NaiveSQL2RelationalAlgebraConverter();
                            converter.SetQueryCalculus(s3);
                            Relation relationalgebra = converter.SQL2RelationalAlgebra(gdd);

                            //TODO 这里作为测试，临时修改，填写ResultName
                            TempModifier tempModifier = new TempModifier(gdd);
                            tempModifier.Modify(relationalgebra);


                            //测试代码
                            string output = (new RelationDebugger()).GetDebugString(relationalgebra);
                            System.Diagnostics.Debug.WriteLine(output);

                            QueryPlanCreator creator = new QueryPlanCreator(gdd);
                            ExecutionPlan gPlan = creator.CreateGlobalPlan(relationalgebra, 0);



                            List<ExecutionPlan> plans = creator.SplitPlan(gPlan);
                            foreach (ExecutionPlan p in plans)
                            {
                                ExecutionPackage package = new ExecutionPackage();
                                package.ID = 0;
                                package.Object = p;
                                package.Type = ExecutionPackage.PackageType.Plan;

                                conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Common.NetworkCommand.PLAN, package);
                            }

                            foreach (ExecutionPlan p in plans)
                            {
                                NetworkPacket returnPackage = conn.GetLocalSiteClient(p.ExecutionSite.Name).Packets.WaitAndRead();
                                DistDBMS.Common.Debug.WriteLine(name + ": Package from " + p.ExecutionSite.Name);
                                if (returnPackage is ServerClientTextObjectPacket
                                    && (returnPackage as ServerClientTextObjectPacket).Object != null)
                                {
                                    if ((returnPackage as ServerClientTextObjectPacket).Object is ExecutionPackage)
                                    {
                                        ExecutionPackage resultPackage = (returnPackage as ServerClientTextObjectPacket).Object
                                            as ExecutionPackage;

                                        //读取结果
                                        if (resultPackage.Type == ExecutionPackage.PackageType.Data)
                                        {
                                            result.Data = resultPackage.Object as Table;
                                            result.Description = "Command executed successfully.";
                                            result.Description += "\r\n" + result.Data.Tuples.Count + " tuples selected";
                                            result.RawQueryTree = relationalgebra;

                                            result.Description += "\r\n";
                                        }
                                    }
                                }
                            }
                            DistDBMS.Common.Debug.WriteLine("ALL plans done!");
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);
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
                                conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Common.NetworkCommand.PLAN, package);
                            }
                            result.Description += "\r\n";

                            foreach (ExecutionPlan p in plans)
                            {
                                
                                NetworkPacket returnPackage = conn.GetLocalSiteClient(p.ExecutionSite.Name).Packets.WaitAndRead();
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
                                conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(newSessionId,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Network.SessionStepPacket.StepIndexNone,
                                        Common.NetworkCommand.PLAN, package);
                            }
                            result.Description += "\r\n";

                            foreach (ExecutionPlan p in plans)
                            {

                                NetworkPacket returnPackage = conn.GetLocalSiteClient(p.ExecutionSite.Name).Packets.WaitAndRead();
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
