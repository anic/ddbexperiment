using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite
{
    public class VirtualInterface
    {
        FragmentList ldd; //用于保存local的数据字典
        string name;
        public VirtualInterface(string siteName)
        {
            ldd = new FragmentList();
            name = siteName;
        }

        public void ReceiveExecutionPackage(ExecutionPackage package)
        { 

        }

        public void ReceiveGdd(GlobalDirectory gdd)
        {
            ldd.AddRange(gdd.Fragments.GetFragmentsBySiteName(name));
        }
    }
}
