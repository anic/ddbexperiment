using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Execution
{
    [Serializable]
    public class ExecutionResult
    {
        public string Description { get; set; }

        public Table.Table Data { get; set; }

        public RelationalAlgebra.Entity.Relation RawQueryTree { get; set; }

        public ExecutionRelation OptimizedQueryTree { get; set; }

        public ExecutionResult()
        {
            Description = "";
            Data = null;
            RawQueryTree = null;
            OptimizedQueryTree = null;
        }
    }
}
