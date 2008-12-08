using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.Common.Execution
{
    public class ExecutionPlan
    {
        /// <summary>
        /// 执行计划的标识，同一个执行计划的标识是一样的
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 执行计划的步骤
        /// </summary>
        public List<ExecutionStep> Steps { get; set; }

        /// <summary>
        /// 在哪个站点执行
        /// </summary>
        public Site ExecutionSite { get; set; }

        public ExecutionPlan()
        {
            Steps = new List<ExecutionStep>();
            ID = "";
            ExecutionSite = null;
        }

        public new string ToString()
        { 
            string result = "";
            if (ExecutionSite != null)
                result += "Site: " + ExecutionSite.Name + "\n";
            int index = 0;
            foreach (ExecutionStep step in Steps)
            {
                result += "Step " + index.ToString() + ": \n" + step.ToString() + "\n";
                index++;
            }
            return result;
        }
    }
}
