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
                foreach (ExecutionPackage package in this)
                    if (package.ID == id)
                        return package;

                return null;
            }
        }
    }
}
