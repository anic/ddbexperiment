using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;

namespace DistDBMS.LocalSite
{
    /// <summary>
    /// 调试用的缓存
    /// </summary>
    public class VirtualBuffer:List<ExecutionPackage>
    {
        public ExecutionPackage GetPackageById(int id)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                ExecutionPackage package = this[i];
                if (package.ID == id)
                    return package;
            }
            return null;
        }

        public ExecutionPlan GetPlan()
        {
            for (int i = 0; i < this.Count; ++i)
            {
                ExecutionPackage package = this[i];
                if (package.Object != null && package.Object is ExecutionPlan)
                    return package.Object as ExecutionPlan;
            }

            return null;
        }

        public new void Add(ExecutionPackage item)
        {
            base.Add(item);
        }
    }

    


}
