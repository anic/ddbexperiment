using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace DistDBMS.Common.Syntax
{
    /// <summary>
    /// 原子条件，更像谓词
    /// </summary>
    public class AtomCondition:ICloneable
    {
        public enum AtomConditionType
        {
            Invariable_True,
            Invariable_False,
            Unary,
            Binary,
            Error
        }

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

        /// <summary>
        /// 谓词类型
        /// 
        /// 类型可以是：一元谓词、二元谓词、恒真式、恒假式
        /// </summary>
        public AtomConditionType Type
        {
            get
            {
                if (LeftOperand.IsField ^ RightOperand.IsField)
                {
                    return AtomConditionType.Unary;
                }
                else if (LeftOperand.IsField)
                {
                    return AtomConditionType.Binary;
                }
                else
                {
                    // 判断恒真式或恒假式
                    // 不考虑隐式类型转换的情况，如 2 > 1.5
                    if (LeftOperand.ValueType != RightOperand.ValueType)
                        return AtomConditionType.Error;
                        
                    bool value;
                    if (LeftOperand.ValueType == AttributeType.Int)
                    {
                        int left = LeftOperand.Value as int;
                        int right = RightOperand.Value as int;

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = (left == right);
                            case LogicOperator.NotEqual:
                                value = (left != right);
                            case LogicOperator.Greater:
                                value = (left > right);
                            case LogicOperator.GreaterOrEqual:
                                value = (left >= right);
                            case LogicOperator.Less:
                                value = (left < right);
                            case LogicOperator.LessOrEqual:
                                value = (left <= right);
                        }
                    }
                    else if (LeftOperand.ValueType == AttributeType.Double)
                    {
                        double left = LeftOperand.Value as double;
                        double right = RightOperand.Value as double;

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = (left == right);
                            case LogicOperator.NotEqual:
                                value = (left != right);
                            case LogicOperator.Greater:
                                value = (left > right);
                            case LogicOperator.GreaterOrEqual:
                                value = (left >= right);
                            case LogicOperator.Less:
                                value = (left < right);
                            case LogicOperator.LessOrEqual:
                                value = (left <= right);
                        }
                        
                        if (value)
                            return AtomConditionType.Invariable_True;
                        else
                            return AtomConditionType.Invariable_False;
                    }
                    else if (LeftOperand.ValueType == AttributeType.String)
                    {
                        string left = LeftOperand.Value as string;
                        string right = RightOperand.Value as string;

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = left.Equals(right);
                            case LogicOperator.NotEqual:
                                value = !(left.Equals(right));
                            default:
                                return AtomConditionType.Error;
                        }
                        if (value)
                            return AtomConditionType.Invariable_True;
                        else
                            return AtomConditionType.Invariable_False;
                    }
                    else
                    {
                        Debug.Assert(false, "Undefined AtomCondition Type");
                        return AtomConditionType.Error;
                    }
                }
            }
        }

        /// <summary>
        /// 对于一元谓词，将其转换为左边为属性，右边为常量的形式
        /// 对于二元谓词，不起任何作用
        /// </summary>
        /// <returns>若为一元谓词，则返回true，否则返回false</returns>
        public bool Normalize()
        {
            if (LeftOperand.IsValue && RightOperand.IsField)
            {
                Operand tmp = RightOperand;
                RightOperand = LeftOperand;
                LeftOperand = tmp;
                return true;
            }
            else if (LeftOperand.IsField && RightOperand.IsValue)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 将二元谓词的左操作数与右操作数调换，并更改操作符方向，反向后谓词意义不变
        /// 对一元谓词不起作用
        /// </summary>
        /// <returns>若为一元谓词，返回false，否则返回true</returns>
        public bool Reverse()
        {
            if (LeftOperand.IsField ^ RightOperand.IsField)
            {
                Operand tmp = LeftOperand;
                LeftOperand = RightOperand;
                RightOperand = tmp;
                
                Operator = AtomCondition.ReverseOperator(Operator);

                return true;
            }
            else
            {
                return false;
            } 
        }

        /// <summary>
        /// 反向操作符
        /// </summary>
        /// <param name="op">待反向操作符</param>
        /// <returns>反向后操作符</returns>
        public static LogicOperator ReverseOperator(LogicOperator op)
        {
            switch (op)
            {
                case LogicOperator.Equal:
                case LogicOperator.NotEqual:
                    return op;

                case LogicOperator.Greater:
                    return LogicOperator.Less;

                case LogicOperator.Less:
                    return LogicOperator.Greater;

                case LogicOperator.GreaterOrEqual:
                    return LogicOperator.LessOrEqual;

                case LogicOperator.LessOrEqual:
                    return LogicOperator.GreaterOrEqual;
            }
        }

        /// <summary>
        /// 与另外一个AtomCondition作逻辑‘与’操作
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool And(AtomCondition other)
        {
            return true;
        }
        
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
