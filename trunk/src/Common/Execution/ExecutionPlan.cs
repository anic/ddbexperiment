using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Execution
{
    public class ExecutionPlan
    {
        public Guid ID { get; set; }

        public List<ExecutionStep> Steps { get; set; }
    }
}
