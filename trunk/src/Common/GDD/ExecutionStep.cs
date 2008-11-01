using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.GDD
{
    public class ExecutionStep
    {
        public int Index { get; set; }

        public Site ExecutionSite { get; set; }

        public List<int> WaitingSteps { get; set; }

        public List<string> ExecutionSQL { get; set; }

        public Site TransferSite { get; set; }

        public Status Status { get; set; }

        //TODO:这里需要斟酌，包括记录是否完成，事件，状态信息
    }
}
