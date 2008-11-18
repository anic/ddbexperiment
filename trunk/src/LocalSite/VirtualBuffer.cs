using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;

namespace DistDBMS.ControlSite
{
    /// <summary>
    /// 调试用的缓存
    /// </summary>
    public class VirtualBuffer:List<ExecutionPackage>
    {
        public ExecutionPackage this[string id]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ExecutionPackage package = this[i];
                    if (package.ID == id)
                        return package;
                }
                return null;
            }
        }

        public static bool Flag = false;
    }

    


}
