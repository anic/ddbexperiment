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
                                conn.GetLocalSiteClient(site.Name).SendStepTextObjectPacket(conn.SessionId,
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
                            conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(conn.SessionId,
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
                    else if ((packet as ServerClientTextObjectPacket).Text == Common.NetworkCommand.EXESQL)
                    {
                        string sql = (packet as ServerClientTextObjectPacket).Object as string;
                        ParserSwitcher ps = new ParserSwitcher();
                        bool bParse = ps.Parse(sql.Trim());
                        ExecutionResult result = new ExecutionResult();
                        if (!bParse)
                        {
                            
                            result.Description = "Parse sql error. \nInfo:" + ps.LastError;
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, result);
                            return;
                        }

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

                            conn.GetLocalSiteClient(p.ExecutionSite.Name).SendStepTextObjectPacket(conn.SessionId,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Network.SessionStepPacket.StepIndexNone,
                                    Common.NetworkCommand.PLAN, package);
                            
                            //ThreadExample ex = new ThreadExample(virInterfaces[p.ExecutionSite.Name] as VirtualInterface, package);
                            //Thread t = new Thread(new ThreadStart(ex.ThreadProc));
                            //threads.Add(ex);
                            //t.Start();
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
                                        //if (result.Data.Tuples.Count > 0)
                                        //{
                                        //    result.Description += ":\r\n";
                                        //    foreach (Tuple tuple in result.Data.Tuples)
                                        //        result.Description += tuple.ToString() + "\r\n";
                                        //}
                                        //else
                                        //    result.Description += "\r\n";
                                        result.Description += "\r\n";
                                    }
                                }
                            }
                        }
                        DistDBMS.Common.Debug.WriteLine("ALL plans done!");
                        conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, result);
                        return;
                    }

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
