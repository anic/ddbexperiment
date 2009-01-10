using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Execution
{
    /// <summary>
    /// 运行结果
    /// </summary>
    [Serializable]
    public class ExecutionResult
    {
        public enum ResultType { 
            Data,
            Command
        }

        /// <summary>
        /// 类型
        /// </summary>
        public ResultType Type { get; set; }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public Table.Table Data { get; set; }

        /// <summary>
        /// 原始关系代数
        /// </summary>
        public RelationalAlgebra.Entity.Relation RawQueryTree { get; set; }

        /// <summary>
        /// 经过查询优化的可执行关系代数树
        /// </summary>
        public ExecutionRelation OptimizedQueryTree { get; set; }

        public ExecutionResult()
        {
            Type = ResultType.Command;
            Description = "";
            Data = null;
            RawQueryTree = null;
            OptimizedQueryTree = null;
        }
    }
}
