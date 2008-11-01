using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.Common;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    /// <summary>
    /// 原子条件，更像谓词
    /// </summary>
    class AtomCondition
    {
        /// <summary>
        /// 操作符
        /// </summary>
        public LogicOperator Operator{get;set;}

        /// <summary>
        /// 左操作数
        /// </summary>
        public Operand LeftOperand { get; set; } //还是说用接口

        /// <summary>
        /// 右操作数
        /// </summary>
        public Operand RightOperand { get; set; }

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

    }
}
