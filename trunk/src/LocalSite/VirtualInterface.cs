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
        FragmentList ldd; //用于保存local的数据字典
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


            ldd = new FragmentList();
            name = siteName;
            this.buffer = buffer;
        }

        public void ReceiveExecutionPackage(ExecutionPackage package)
        {
            if (package.Type == ExecutionPackage.PackageType.Plan)
            {
                ExecutionPlan plan = package.Object as ExecutionPlan;
                QueryProcessor processor = new QueryProcessor();
                foreach (ExecutionStep step in plan.Steps)
                {
                    if (step.Type == ExecutionStep.ExecuteType.Select)
                    {
                        while (true)
                        {
                            bool allReady = true;
                            foreach (string id in step.WaitingId)
                                allReady &= (buffer[id] != null);

                            if (allReady)
                                break;

                            Thread.Sleep(1000); //现在是停等，以后应该是异步等，或者唤醒机制
                        }

                        Table table = processor.Handle(step, name, buffer);

                        //相当于发数据
                        ExecutionPackage newPackage = new ExecutionPackage();
                        package.ID = step.Operation.ResultID;
                        package.Type = ExecutionPackage.PackageType.Data;
                        package.Object = table;

                        lock (buffer)
                        {
                            buffer.Add(package);//相当于异步发送
                        }
                    }
                    else if (step.Type == ExecutionStep.ExecuteType.Insert)
                    {
                        using (DataAccess.DataAccessor da = new DistDBMS.ControlSite.DataAccess.DataAccessor(name))
                        {
                            da.InsertValues(step.Table);
                        }
                    }
                }
            }
        }

        public void ReceiveGdd(GlobalDirectory gdd)
        {
            ldd.AddRange(gdd.Fragments.GetFragmentsBySiteName(name));

            using (DataAccess.DataAccessor da = new DistDBMS.ControlSite.DataAccess.DataAccessor(name))
            { 
                foreach(Fragment f in ldd)
                {
                    TableSchema localTable = f.Schema.Clone() as TableSchema;
                    localTable.TableName = f.LogicTable.TableName;
                    da.CreateTable(localTable);
                }
            }
        }
    }
}
