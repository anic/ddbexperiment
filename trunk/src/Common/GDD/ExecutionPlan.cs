using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.GDD
{
    public class ExecutionPlan
    {
        public Guid ID { get; set; }

        public List<ExecutionStep> Steps { get; set; }
    }
}
