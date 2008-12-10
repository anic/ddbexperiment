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
        public PackageProcessor(string localSiteName)
        {
            this.name = localSiteName;
            this.buffer = new VirtualBuffer();
        }

        LocalDirectory ldd;

        public void LocalSitePackageProcess(LocalSiteServerConnection conn, LocalSiteServerPacket packet)
        {
            System.Console.WriteLine(name + ":Packet received");

            if (packet is LocalSiteServerTextObjectPacket)
            {
                if ((packet as LocalSiteServerTextObjectPacket).Text == Common.NetworkCommand.PLAN)
                {
                    ExecutionPackage package = (packet as LocalSiteServerTextObjectPacket).Object as ExecutionPackage;
                    
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
                            conn.SendServerClientTextPacket("BAD");
                    }
                    else if (package.Type == ExecutionPackage.PackageType.Plan)//执行计划
                    {
                        ExecutionPlan plan = package.Object as ExecutionPlan;
                        QueryProcessor processor = new QueryProcessor(ldd);
                        foreach (ExecutionStep step in plan.Steps)
                        {
                            if (step.Type == ExecutionStep.ExecuteType.Select)
                            {
                                while (true)
                                {
                                    bool allReady = true;
                                    lock (buffer)
                                    {
                                        for (int i = 0; i < step.WaitingId.Count; i++)
                                            allReady &= (buffer.GetPackageById(step.WaitingId[i]) != null);

                                        if (allReady)
                                            break;
                                    }

                                    Thread.Sleep(new Random().Next(1000)); //现在是停等，以后应该是异步等，或者唤醒机制
                                }

                                lock (buffer)
                                {
                                    Table table = processor.Handle(step, name, buffer);
                                    //TODO:做异常处理
                                    //if(table==null)

                                    ExecutionPackage newPackage = new ExecutionPackage();
                                    newPackage.ID = step.Operation.ResultID;
                                    newPackage.Type = ExecutionPackage.PackageType.Data;
                                    newPackage.Object = table;
                                    buffer.Add(newPackage);//相当于异步发送
                                    if (step.TransferSite != null)
                                        conn.SendP2PStepTextObjectPacket(step.TransferSite.Name,
                                            Network.SessionStepPacket.StepIndexNone,
                                            Common.NetworkCommand.EXESQL,
                                            newPackage);
                                    else if (step.Index == 0) //返回ControlSite
                                        conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, newPackage);
                                    else
                                        buffer.Add(newPackage);
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
                        conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
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
                    lock (buffer)
                    {
                        buffer.Add((packet as P2PTextObjectPacket).Object as ExecutionPackage);
                    }
                }
            }
        }

    }
}
