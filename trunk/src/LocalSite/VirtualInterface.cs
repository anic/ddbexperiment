using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.Processor;
using System.IO;
using DistDBMS.Common.Table;
using System.Threading;

namespace DistDBMS.ControlSite
{
    public class VirtualInterface
    {
        LocalDirectory ldd; //用于保存local的数据字典
        string name;

        VirtualBuffer buffer;

        /// <summary>
        /// 用于调试的平台，虚拟接口
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="buffer"></param>
        public VirtualInterface(string siteName,VirtualBuffer buffer)
        {
            try
            {
                File.Delete(siteName);
            }
            catch (Exception ex) {
                System.Console.WriteLine(ex);
            }


            
            name = siteName;
            this.buffer = buffer;
        }

        public void ReceiveExecutionPackage(ExecutionPackage package)
        {
            if (package.Type == ExecutionPackage.PackageType.Plan)
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

                        
                        

                        //相当于发数据
                        
                        lock (buffer)
                        {
                            Table table = processor.Handle(step, name, buffer);
                            ExecutionPackage newPackage = new ExecutionPackage();
                            newPackage.ID = step.Operation.ResultID;
                            newPackage.Type = ExecutionPackage.PackageType.Data;
                            newPackage.Object = table;
                            buffer.Add(newPackage);//相当于异步发送
                        }
                    }
                    else if (step.Type == ExecutionStep.ExecuteType.Insert)
                    {
                        using (DataAccess.DataAccessor da = new DistDBMS.ControlSite.DataAccess.DataAccessor(name))
                        {
                            TableSchema logicSchema = ldd.Fragments.GetFragmentByName(step.Table.Name).LogicSchema;
                            step.Table.Schema.ReplaceTableName(logicSchema.TableName);
                            int result = da.InsertValues(step.Table);
                        }
                    }
                }
            }
        }

        public void ReceiveGdd(GlobalDirectory gdd)
        {
            ldd = new LocalDirectory(gdd, name);

            using (DataAccess.DataAccessor da = new DistDBMS.ControlSite.DataAccess.DataAccessor(name))
            { 
                foreach(Fragment f in ldd.Fragments)
                {
                    TableSchema localTable = f.Schema.Clone() as TableSchema;
                    localTable.TableName = f.LogicSchema.TableName;
                    da.CreateTable(localTable);
                }
            }
        }
    }
}
