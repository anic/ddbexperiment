using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.Common;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Condition
    {
        /// <summary>
        /// 连接符，And / Or
        /// </summary>
        public RelationOperator Operator{get;set;}

        /// <summary>
        /// 左条件
        /// </summary>
        public Condition LeftCondition { get; set; }

        /// <summary>
        /// 右条件
        /// </summary>
        public Condition RightCondition { get; set; }

        /// <summary>
        /// 是否原子条件，即是否有连接符
        /// </summary>
        public bool IsAtomCondition { get; set; }

        /// <summary>
        /// 原子条件
        /// </summary>
        public AtomCondition AtomCondition { get; set; }

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public Condition()
        {
            Operator = RelationOperator.And;
            LeftCondition = null;
            RightCondition = null;
            IsAtomCondition = false;
            AtomCondition = new AtomCondition();
            Content = "";
        }

    }
}
