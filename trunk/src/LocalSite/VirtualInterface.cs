using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.Processor;
using System.IO;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite
{
    public class VirtualInterface
    {
        FragmentList ldd; //用于保存local的数据字典
        string name;
        public VirtualInterface(string siteName)
        {
            try
            {
                File.Delete(siteName);
            }
            catch (Exception ex) { }


            ldd = new FragmentList();
            name = siteName;
        }

        public void ReceiveExecutionPackage(ExecutionPackage package)
        {
            if (package.Type == ExecutionPackage.PackageType.Plan)
            {
                ExecutionPlan plan = package.Object as ExecutionPlan;
                QueryProcessor processor = new QueryProcessor();
                foreach (ExecutionStep step in plan.Steps)
                    processor.Handle(step, name);
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
