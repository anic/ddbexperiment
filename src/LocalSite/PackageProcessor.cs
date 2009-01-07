using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Network;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.Common.Execution;
using DistDBMS.LocalSite.Processor;
using System.Threading;

namespace DistDBMS.LocalSite
{
    class PackageProcessor
    {
        string name;
        public string Name { get { return name; } }
        VirtualBuffer buffer;
        ExecutionPlan currentPlan;
        LocalDirectory ldd;
        public PackageProcessor(string localSiteName)
        {
            this.name = localSiteName;
            this.buffer = new VirtualBuffer();

            //等待数据来才初始化
            this.currentPlan = null;
            this.ldd = null;
        }


        public void LocalSitePackageProcess(LocalSiteServerConnection conn, LocalSiteServerPacket packet)
        {
            System.Console.WriteLine(name + ":Packet received");

            if (packet is LocalSiteServerTextObjectPacket)
            {
                if ((packet as LocalSiteServerTextObjectPacket).Text == Common.NetworkCommand.PLAN)
                {
                    ExecutionPackage package = (packet as LocalSiteServerTextObjectPacket).Object as ExecutionPackage;
                    System.Console.WriteLine(name + " package ID:" + package.ID);
                    if (package.Type == ExecutionPackage.PackageType.Gdd)//gdd
                    {
                        bool result = true;
                        ldd = new LocalDirectory(package.Object as GlobalDirectory, name);
                        using (DataAccess.DataAccessor da = new DistDBMS.LocalSite.DataAccess.DataAccessor(name))
                        {
                            foreach (Fragment f in ldd.Fragments)
                            {
                                TableSchema localTable = f.Schema.Clone() as TableSchema;
                                localTable.TableName = f.LogicSchema.TableName;
                                result &= da.CreateTable(localTable);
                            }
                        }

                        if (result)
                            conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                        else
                            conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_ERROR);
                    }
                    else if (package.Type == ExecutionPackage.PackageType.Plan)//执行计划
                    {
                        ExecutionPlan plan = package.Object as ExecutionPlan;
                        if (currentPlan == null)
                        {
                            currentPlan = plan;
                            //能执行多少执行多少
                            ExecutePlan(conn);
                        }
                        else
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, "BUSY");
                    }
                }
            }
        }

        public void P2PPackageProcess(LocalSiteServerConnection conn, P2PPacket packet)
        {
            System.Console.WriteLine(name + ":P2P Packet received");
            if (packet is P2PTextObjectPacket)
            { 
                if ((packet as P2PTextObjectPacket).Object is ExecutionPackage)
                {
                    System.Console.WriteLine(name + " package ID:" + ((packet as P2PTextObjectPacket).Object as ExecutionPackage).ID);
                    lock (buffer)
                        buffer.Add((packet as P2PTextObjectPacket).Object as ExecutionPackage);

                    ExecutePlan(conn);
                }
            }
        }

        private void ExecutePlan(LocalSiteServerConnection conn)
        {
            ExecutionPlan plan = currentPlan;
            if (plan == null)
                return;

            QueryProcessor processor = new QueryProcessor(ldd);
            bool allDone = true;
            foreach (ExecutionStep step in plan.Steps)
            {
                if (step.Type == ExecutionStep.ExecuteType.Select)
                {
                    if (step.Status!=null && step.Status.Done) //步骤已经完成
                        continue;

                    bool allWaitedPackageArrived = true; //所有等待的包都到齐了
                    lock (buffer)
                    {
                        //TODO:现在在我的Buffer找，以后在Packect中找
                        for (int i = 0; i < step.WaitingId.Count; i++)
                            allWaitedPackageArrived &= (buffer.GetPackageById(step.WaitingId[i]) != null);

                        //如果数据到齐了，执行，否则执行下一个步骤
                        if (allWaitedPackageArrived)
                        {
                            step.Status = new Status();
                            step.Status.StartTime = DateTime.Now;

                            Table table = processor.Handle(step, name, buffer);
                            //TODO:做异常处理
                            //if(table==null)

                            step.Status.EndTime = DateTime.Now;
                            step.Status.Done = true;

                            //产生一个新的Data的包
                            ExecutionPackage newPackage = new ExecutionPackage();
                            newPackage.ID = step.Operation.ResultID;
                            newPackage.Type = ExecutionPackage.PackageType.Data;
                            newPackage.Object = table;

                            buffer.Add(newPackage);//相当于异步发送
                            if (step.TransferSite != null && step.TransferSite.Name != this.name)
                            {
                                conn.SendP2PStepTextObjectPacket(step.TransferSite.Name,
                                    newPackage.ID,
                                    Common.NetworkCommand.EXESQL,
                                    newPackage);                                
                            }
                            else if (step.Index == 0) //返回ControlSite
                            {
                                System.Diagnostics.Debug.WriteLine(name + " finish the plan");
                                System.Console.WriteLine(name + " finish the plan");
                                conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, newPackage);
                            }
                            else
                                buffer.Add(newPackage);

                        }
                        else
                            allDone = false;
                    }
                }
                else if (step.Type == ExecutionStep.ExecuteType.Insert)
                {
                    using (DataAccess.DataAccessor da = new DistDBMS.LocalSite.DataAccess.DataAccessor(name))
                    {
                        TableSchema logicSchema = ldd.Fragments.GetFragmentByName(step.Table.Name).LogicSchema;
                        step.Table.Schema.ReplaceTableName(logicSchema.TableName);
                        int result = da.InsertValues(step.Table);
                    }
                }
            }

            if (allDone)
            {
                System.Diagnostics.Debug.WriteLine(name + " finish the plan");
                System.Console.WriteLine(name + " finish the plan");
                conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                currentPlan = null;
                lock (buffer)
                    buffer.Clear();
            }
        }

    }
}
