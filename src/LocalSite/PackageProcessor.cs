using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Network;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.Common.Execution;
using DistDBMS.LocalSite.Processor;
using System.Threading;
using System.IO;

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

        public void Init()
        {
            //if (File.Exists(this.name))
            //{
            //    using (DataAccess.DataAccessor da = new DistDBMS.LocalSite.DataAccess.DataAccessor(name))
            //    {
            //        //从数据库中获取ldd
            //        TableSchema lddTable = CreateLddTable();
            //        this.ldd = da.QueryLdd(lddTable) as LocalDirectory;
            //    }
            //}
        }

        private TableSchema CreateLddTable()
        {
            TableSchema lddTable = new TableSchema();
            Field fId = new Field();
            fId.AttributeName = "id";
            fId.AttributeType = DistDBMS.Common.AttributeType.Int;
            Field fContent = new Field();
            fContent.AttributeName = "content";
            fContent.AttributeType = DistDBMS.Common.AttributeType.BLOB;
            lddTable.Fields.Add(fId);
            lddTable.Fields.Add(fContent);
            lddTable.ReplaceTableName("LddTable");

            return lddTable;
        }


        public void LocalSitePackageProcess(LocalSiteServerConnection conn, LocalSiteServerPacket packet)
        {
            DistDBMS.Common.Debug.WriteLine(name + ":Packet received");

            if (packet is LocalSiteServerTextObjectPacket)
            {
                if ((packet as LocalSiteServerTextObjectPacket).Text == Common.NetworkCommand.PLAN)
                {
                    ExecutionPackage package = (packet as LocalSiteServerTextObjectPacket).Object as ExecutionPackage;
                    DistDBMS.Common.Debug.WriteLine(name + " package ID:" + package.ID);
                    if (package.Type == ExecutionPackage.PackageType.Gdd)//gdd
                    {
                        bool result = true;
                        ldd = new LocalDirectory(package.Object as GlobalDirectory, name);
                        string error = string.Empty;
                        using (DataAccess.DataAccessor da = new DistDBMS.LocalSite.DataAccess.DataAccessor(name))
                        {
                            foreach (Fragment f in ldd.Fragments)
                            {
                                TableSchema localTable = f.Schema.Clone() as TableSchema;
                                localTable.TableName = f.LogicSchema.TableName;
                                result &= da.CreateTable(localTable);
                            }

                            //TableSchema lddTable = CreateLddTable();
                            //da.DropTable(lddTable.TableName);
                            //da.CreateTable(lddTable);
                            //da.InsertLdd(lddTable, ldd);

                            if (!result)
                                error = da.LastException.Message;
                        }

                        if (result)
                            conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                        else
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, error);
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
                    else if (package.Type == ExecutionPackage.PackageType.PlanData)//执行计划
                    {
                        ExecutionPlan plan = package.Object as ExecutionPlan;
                        int tables = packet.ReadInt();
                        for (int i = 0; i < tables; ++i)
                        { 
                            int size = packet.ReadInt();
                            for (int j = 0; j < size; ++j)
                                plan.Steps[i].Table.Tuples.Add(Tuple.FromLineString(packet.ReadString()));
                        }
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
            DistDBMS.Common.Debug.WriteLine(name + ":P2P Packet received");
            if (packet is P2PTextObjectPacket)
            { 
                if ((packet as P2PTextObjectPacket).Object is ExecutionPackage)
                {
                    ExecutionPackage exPackage = (packet as P2PTextObjectPacket).Object as ExecutionPackage;
                    //恢复数据包
                    if (exPackage.Type == ExecutionPackage.PackageType.Data)
                    {
                        int size = packet.ReadInt();
                        Table t = new Table(size);
                        t.Schema = (exPackage.Object as Table).Schema;
                        exPackage.Object = t;
                        for (int i = 0; i < size; ++i)
                            t.Tuples.Add(Tuple.FromLineString(packet.ReadString()));
                        Common.Debug.WriteLine("---------------Received tuple:" + size);
                    }

                    DistDBMS.Common.Debug.WriteLine(name + " package ID:" + ((packet as P2PTextObjectPacket).Object as ExecutionPackage).ID);
                    lock (buffer)
                        buffer.Add(exPackage);

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
                            Table table;
                            //TODO:做异常处理
                            try
                            {
                                if (step.Operation.ResultID == 100)
                                {
                                    int a = 0;
                                }
                                table = processor.Handle(step, name, buffer);
                            }
                            catch{
                                table = new Table();
                            }

                            step.Status.EndTime = DateTime.Now;
                            step.Status.Done = true;
                            TimeSpan ts = step.Status.EndTime - step.Status.StartTime;
                            Common.Debug.WriteLine("step" + step.Index + " :" + ts.TotalMilliseconds + " ms");

                            //产生一个新的Data的包
                            ExecutionPackage newPackage = new ExecutionPackage();
                            newPackage.ID = step.Operation.ResultID;
                            newPackage.Type = ExecutionPackage.PackageType.Data;
                            newPackage.Object = new Table();
                            (newPackage.Object as Table).Schema = table.Schema;
                            
                            //TODO:应该检查是否在本站点
                            
                            if (step.TransferSite != null && step.TransferSite.Name != this.name)
                            {
                                P2PTextObjectPacket packet = conn.EncapsulateP2PStepTextObjectPacket(newPackage.ID,
                                    Common.NetworkCommand.EXESQL,
                                    newPackage);

                                packet.EnsureSize(10*1024*1024);
                                StringBuilder sb = new StringBuilder(10 * 1024);
                                packet.WriteInt(table.Tuples.Count); //先写数据大小
                                foreach (Tuple t in table.Tuples)
                                    packet.WriteString(t.GenerateLineString(sb));
                                
                                Common.Debug.WriteLine("---------------Write tuple:" + table.Tuples.Count);
                                conn.SendP2PStepTextObjectPacket(step.TransferSite.Name, packet);
;                                
                            }
                            else if (step.Index == 0) //返回ControlSite
                            {
                                System.Diagnostics.Debug.WriteLine(name + " finish the plan");
                                DistDBMS.Common.Debug.WriteLine(name + " finish the plan");

                                ServerClientPacket packet = conn.EncapsulateServerClientTextObjectPacket(Common.NetworkCommand.RESULT_OK, newPackage);
                                Common.Debug.WriteLine("---------------Write tuple:" + table.Tuples.Count);
                                packet.EnsureSize(10 * 1024 * 1024);
                                StringBuilder sb = new StringBuilder(10 * 1024);
                                packet.WriteInt(table.Tuples.Count); //先写数据大小
                                foreach (Tuple t in table.Tuples)
                                    packet.WriteString(t.GenerateLineString(sb));

                                conn.SendPacket(packet);
                            }
                            else
                            {
                                newPackage.Object = table;
                                buffer.Add(newPackage);
                            }

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

                        if (result == -1)
                        {
                            DistDBMS.Common.Debug.WriteLine(name + " fail the insert");
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, da.LastException.Message);
                        }
                        else
                        {
                            DistDBMS.Common.Debug.WriteLine(name + " finish the insert");
                            conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                        }
                            
                    }
                }
                else if (step.Type == ExecutionStep.ExecuteType.Delete)
                {
                    using (DataAccess.DataAccessor da = new DistDBMS.LocalSite.DataAccess.DataAccessor(name))
                    {
                        TableSchema logicSchema = ldd.Fragments.GetFragmentByName(step.Operation.DirectTableSchema.TableName).LogicSchema;
                        step.Operation.DirectTableSchema.ReplaceTableName(logicSchema.TableName);
                        int result = da.DeleteValue(step.Operation.DirectTableSchema, step.Operation.Predication);
                        if (result == -1)
                        {
                            DistDBMS.Common.Debug.WriteLine(name + " fail the delete");
                            conn.SendServerClientTextObjectPacket(Common.NetworkCommand.RESULT_ERROR, da.LastException.Message);
                        }
                        else
                        {
                            DistDBMS.Common.Debug.WriteLine(name + " finish the delete");
                            conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                        }

                    }
                }
            }

            if (allDone)
            {
                System.Diagnostics.Debug.WriteLine(name + " finish the plan");
                DistDBMS.Common.Debug.WriteLine(name + " finish the plan");
                conn.SendServerClientTextPacket(Common.NetworkCommand.RESULT_OK);
                currentPlan = null;
                lock (buffer)
                    buffer.Clear();
            }
        }

    }
}
