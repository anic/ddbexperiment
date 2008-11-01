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
        public Operand LeftOperand { get { return left; } }
        Operand left;

        /// <summary>
        /// 右操作数
        /// </summary>
        public Operand RightOperand { get { return right; } }
        Operand right;

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public AtomCondition()
        {
            Operator = LogicOperator.Equal;
            left = new Operand();
            right = new Operand();
            Content = "";
        }

        public new string ToString()
        { 
            string op = "";
            switch(Operator)
            {
                case LogicOperator.Equal:
                    op = " = ";
                    break;
                case LogicOperator.Greater:
                    op = " > ";
                    break;
                case LogicOperator.GreaterOrEqual:
                    op = " >= ";
                    break;
                case LogicOperator.Less:
                    op = " < ";
                    break;
                case LogicOperator.LessOrEqual:
                    op = " <= ";
                    break;
                case LogicOperator.NotEqual:
                    op = " <> ";
                    break;
                default:
                    op = " error ";
                    break;
            }

            return LeftOperand.ToString() + op + RightOperand.ToString();
        }

    }
}
