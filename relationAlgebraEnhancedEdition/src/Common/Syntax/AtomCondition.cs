using System;
using System.Collections.Generic;
using System.Text;


namespace DistDBMS.Common.Syntax
{
    /// <summary>
    /// 原子条件，更像谓词
    /// </summary>
    public class AtomCondition:ICloneable
    {
        /// <summary>
        /// 操作符
        /// </summary>
        public LogicOperator Operator{get;set;}

        /// <summary>
        /// 左操作数
        /// </summary>
        public Operand LeftOperand { get; set; }
        

        /// <summary>
        /// 右操作数
        /// </summary>
        public Operand RightOperand { get; set; }
        
        public AtomCondition()
        {
            Operator = LogicOperator.Equal;
            LeftOperand = new Operand();
            RightOperand = new Operand();
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


        #region ICloneable Members

        public object Clone()
        {
            AtomCondition result = new AtomCondition();
            result.LeftOperand = LeftOperand.Clone() as Operand;
            result.RightOperand = RightOperand.Clone() as Operand;
            result.Operator = Operator;
            return result;
        }

        #endregion
    }
}
