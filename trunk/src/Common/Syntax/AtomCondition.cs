using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace DistDBMS.Common.Syntax
{
    /// <summary>
    /// 原子条件，更像谓词
    /// </summary>
    [Serializable]
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
        /// 谓词优先级 (优化时使用)
        /// </summary>
        public int Priority { get; set; }

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
                        
                    bool value = false;
                    if (LeftOperand.ValueType == AttributeType.Int)
                    {
                        int left = LeftOperand.ToIntValue();
                        int right = RightOperand.ToIntValue();

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = (left == right);
                                break;
                            case LogicOperator.NotEqual:
                                value = (left != right);
                                break;
                            case LogicOperator.Greater:
                                value = (left > right);
                                break;
                            case LogicOperator.GreaterOrEqual:
                                value = (left >= right);
                                break;
                            case LogicOperator.Less:
                                value = (left < right);
                                break;
                            case LogicOperator.LessOrEqual:
                                value = (left <= right);
                                break;
                        }
                        if (value)
                            return AtomConditionType.Invariable_True;
                        else
                            return AtomConditionType.Invariable_False;
                    }
                    else if (LeftOperand.ValueType == AttributeType.Double)
                    {
                        double left = LeftOperand.ToDoubleValue();
                        double right = RightOperand.ToDoubleValue();

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = (left == right);
                                break;
                            case LogicOperator.NotEqual:
                                value = (left != right);
                                break;
                            case LogicOperator.Greater:
                                value = (left > right);
                                break;
                            case LogicOperator.GreaterOrEqual:
                                value = (left >= right);
                                break;
                            case LogicOperator.Less:
                                value = (left < right);
                                break;
                            case LogicOperator.LessOrEqual:
                                value = (left <= right);
                                break;
                        }
                        
                        if (value)
                            return AtomConditionType.Invariable_True;
                        else
                            return AtomConditionType.Invariable_False;
                    }
                    else if (LeftOperand.ValueType == AttributeType.String)
                    {
                        string left = LeftOperand.ToStringValue();
                        string right = RightOperand.ToStringValue();

                        switch (Operator)
                        {
                            case LogicOperator.Equal:
                                value = left.Equals(right);
                                break;
                            case LogicOperator.NotEqual:
                                value = !(left.Equals(right));
                                break;
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
            if (LeftOperand.IsField && RightOperand.IsField)
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

            Debug.Assert(false, "Undefined Operator");
            return LogicOperator.Equal;
        }

        
        /// <summary>
        /// 检测是否与另一个AtomCodition冲突
        /// 
        /// 实际上是两个谓词作逻辑‘与’操作，当两个谓词都是非二元谓词且具有可比性时，才检查冲突。
        /// 
        /// 可比性是指两个谓词均为非二元谓词，且当两个谓词都为一元谓词时，其两个谓词中的属性（表名、属性名）相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns>是否冲突，冲突则返回true，否则返回false</returns>
        public bool ConflictWith(AtomCondition other)
        {
            List<AtomCondition> blankRef = null;
            return ConflictWith(other, ref blankRef);
        }
        
        /// <summary>
        /// 检测是否与另一个AtomCodition冲突
        /// 
        /// 实际上是两个谓词作逻辑‘与’操作，当两个谓词都是非二元谓词且具有可比性时，才检查冲突。
        /// 
        /// 可比性是指两个谓词均为非二元谓词，且当两个谓词都为一元谓词时，其两个谓词中的属性（表名、属性名）相同
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result">若没有冲突则返回化简后的结果</param>
        /// <returns>是否冲突，冲突则返回true，否则返回false</returns>
        public bool ConflictWith(AtomCondition other, ref List<AtomCondition> reducedCondition)
        {
            if (Type == AtomConditionType.Invariable_False || other.Type == AtomConditionType.Invariable_False)
            {
                return true;
            }
            else if (Type == AtomConditionType.Invariable_True)
            {
                if (other.Type == AtomConditionType.Invariable_False)
                    return true;

                return false;
            }
            else if (Type == AtomConditionType.Unary && other.Type == AtomConditionType.Unary)
            {
                // 正规化一元谓词
                Normalize();
                other.Normalize();

                // 可比的两个一元谓词
                if (LeftOperand.Field.Equals(other.LeftOperand.Field, true))
                {
                    // 两个相同的一元谓词是否冲突
                    reducedCondition = new List<AtomCondition>();
                    return UnaryConditionConflict(other, ref reducedCondition);
                }
                else
                    return false;
            }
            else
            {
                Debug.Assert(false, "Undefined AtomCondition in Conflict");
            }
            return true;
        }

        /// <summary>
        /// 比较两个一元谓词是否冲突
        /// </summary>
        /// <param name="other">另一个一元谓词</param>
        /// <param name="result">若没有冲突则返回化简后的结果</param>
        /// <returns>是否冲突，若存在冲突则返回true, 否则返回false</returns>
        private bool UnaryConditionConflict(AtomCondition other, ref List<AtomCondition> result)
        {
            //////////////////////////////////////////////////////
            //
            // 函数正确运行条件:
            //
            //    两个一元谓词已经过正规化且两个一元谓词的中的属性为相同属性
            //

            // 字符串类型
            if (RightOperand.ValueType == AttributeType.String)
            {
                Debug.Assert(Operator == LogicOperator.Equal || Operator == LogicOperator.NotEqual, "Error String operator!");
                Debug.Assert(other.Operator == LogicOperator.Equal || other.Operator == LogicOperator.NotEqual, "Error String operator!");
                
                if (RightOperand.ToStringValue().Equals(other.RightOperand.ToStringValue()))
                {
                    if (Operator == other.Operator)
                    {
                        result.Add(Clone() as AtomCondition);
                        
                        return false;
                    }
                    else
                    {
                        result.Add(Clone() as AtomCondition);
                        result.Add(other.Clone() as AtomCondition);
                        return true;
                    }
                }
            }
            else if (RightOperand.ValueType == AttributeType.Int || RightOperand.ValueType == AttributeType.Double)
            {
                double value1 = RightOperand.ToDoubleValue();
                double value2 = other.RightOperand.ToDoubleValue();

                switch (Operator)
                {
                    case LogicOperator.Greater:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                                AtomCondition atom = Clone() as AtomCondition;
                                atom.RightOperand.Value = Math.Max(value1, value2);
                                result.Add(atom);
                                return false;
                            case LogicOperator.GreaterOrEqual:
                                if (value1 > value2)
                                    result.Add(Clone() as AtomCondition);
                                else
                                    result.Add(other.Clone() as AtomCondition);
                                
                                return false;
                            case LogicOperator.Less:
                            case LogicOperator.LessOrEqual:
                                if (value1 >= value2)
                                    return true;
                                else
                                {
                                    result.Add(Clone() as AtomCondition);
                                    result.Add(other.Clone()  as AtomCondition);
                                    return false;
                                }
                            case LogicOperator.NotEqual:
                                if (value1 > value2)
                                {
                                    result.Add(other.Clone() as AtomCondition);    
                                }
                                result.Add(this.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Equal:
                                if (value1 >= value2)
                                    return true;
                                else
                                {
                                    result.Add(this.Clone() as AtomCondition);
                                    result.Add(other.Clone() as AtomCondition);
                                    return false;
                                }
                        }
                        break;
                    case LogicOperator.GreaterOrEqual:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                                if (value1 > value2)
                                    result.Add(this.Clone() as AtomCondition);
                                else
                                    result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.GreaterOrEqual:
                                {
                                    AtomCondition atom = Clone() as AtomCondition;
                                    atom.RightOperand.Value = Math.Max(value1, value2);
                                    result.Add(atom);
                                    return false;
                                }
                            case LogicOperator.Less:
                                if (value1 >= value2)
                                    return true;
                                result.Add(this.Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.LessOrEqual:
                                if (value1 > value2)
                                    return true;
                                else if (value1 == value2)
                                {
                                    AtomCondition atom = Clone() as AtomCondition;
                                    atom.Operator = LogicOperator.Equal;
                                    result.Add(atom);
                                    return false;
                                }
                                else
                                {
                                    result.Add(Clone() as AtomCondition);
                                    result.Add(other.Clone() as AtomCondition);
                                    return false;
                                }
                            case LogicOperator.NotEqual:
                                if (value1 > value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                }
                                else
                                {
                                    result.Add(Clone() as AtomCondition);
                                    result.Add(other.Clone() as AtomCondition);
                                }
                                return false;
                            case LogicOperator.Equal:
                                if (value1 > value2)
                                    return true;

                                result.Add(other.Clone() as AtomCondition);
                                return false;
                        }
                        break;
                    case LogicOperator.Less:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                            case LogicOperator.GreaterOrEqual:
                                if (value1 <= value2)
                                    return true;

                                result.Add(Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Less:
                                AtomCondition atom = Clone() as AtomCondition;
                                atom.RightOperand.Value = Math.Max(value1, value2);
                                result.Add(atom);
                                return false;
                            case LogicOperator.LessOrEqual:
                                if (value1 <= value2)
                                    result.Add(Clone() as AtomCondition);
                                else
                                    result.Add(other.Clone() as AtomCondition);

                                return false;
                            case LogicOperator.NotEqual:
                                if (value1 > value2)
                                    result.Add(other.Clone() as AtomCondition);
                                
                                result.Add(Clone() as AtomCondition);

                                return false;
                            case LogicOperator.Equal:
                                if (value1 > value2)
                                {
                                    result.Add(other.Clone() as AtomCondition);
                                    return false;
                                }

                                return false;
                        }
                        break;
                    case LogicOperator.LessOrEqual:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                                if (value1 <= value2)
                                    return true;

                                result.Add(Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.GreaterOrEqual:
                                if (value1 < value2)
                                    return true;
                                
                                if (value1 > value2)
                                    result.Add(other.Clone() as AtomCondition);
                                
                                result.Add(Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Less:
                                if (value1 < value2)
                                    result.Add(Clone() as AtomCondition);
                                else
                                    result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.LessOrEqual:
                                AtomCondition atom = Clone() as AtomCondition;
                                atom.RightOperand.Value = Math.Min(value1, value2);
                                result.Add(atom);
                                return false;
                            case LogicOperator.NotEqual:
                                if (value1 >= value2)
                                    result.Add(other.Clone() as AtomCondition);

                                result.Add(Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Equal:
                                if (value1 >= value2)
                                {
                                    result.Add(other.Clone() as AtomCondition);
                                    return false;
                                }

                                return true;
                        }
                        break;
                    case LogicOperator.NotEqual:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                                if (value1 > value2)
                                    result.Add(Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.GreaterOrEqual:
                                if (value1 < value2)
                                    return true;

                                if (value1 > value2)
                                {
                                    AtomCondition atom = other.Clone() as AtomCondition;
                                    atom.Operator = LogicOperator.Greater;
                                    result.Add(atom);
                                }
                                else
                                {
                                    result.Add(Clone() as AtomCondition);
                                    result.Add(other.Clone() as AtomCondition);
                                }                                
                                return false;
                            case LogicOperator.Less:
                                if (value1 < value2)
                                    result.Add(Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.LessOrEqual:
                                if (value1 > value2)
                                    return true;

                                if (value1 == value2)
                                {
                                    AtomCondition atom = other.Clone() as AtomCondition;
                                    atom.Operator = LogicOperator.Less;
                                    result.Add(atom);
                                }
                                else
                                {
                                    result.Add(Clone() as AtomCondition);
                                    result.Add(other.Clone() as AtomCondition);
                                }
                                return false;
                            case LogicOperator.NotEqual:
                                if (value1 != value2)
                                    result.Add(Clone() as AtomCondition);
                                result.Add(other.Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Equal:
                                if (value1 == value2)
                                    return true;

                                result.Add(other.Clone() as AtomCondition);
                                return false;
                        }
                        break;
                    case LogicOperator.Equal:
                        switch (other.Operator)
                        {
                            case LogicOperator.Greater:
                                if (value1 > value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                    return false;
                                }
                                else
                                    return true;
                            case LogicOperator.GreaterOrEqual:
                                if (value1 >= value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                    return false;
                                }
                                else
                                    return true;
                            case LogicOperator.Less:
                                if (value1 < value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                    return false;
                                }
                                else
                                    return true;
                            case LogicOperator.LessOrEqual:
                                if (value1 <= value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                    return false;
                                }
                                else
                                    return true;
                            case LogicOperator.NotEqual:
                                if (value1 == value2)
                                    return false;

                                result.Add(Clone() as AtomCondition);
                                return false;
                            case LogicOperator.Equal:
                                if (value1 == value2)
                                {
                                    result.Add(Clone() as AtomCondition);
                                    return false;
                                }
                                else
                                    return true;
                        }
                        break;
                }
            }
            else
            {
                Debug.Assert(false, "Can not handle the ValueType in UnaryConditionConflict");
            }

            return true;
        }
        
        public AtomCondition()
        {
            Operator = LogicOperator.Equal;
            LeftOperand = new Operand();
            RightOperand = new Operand();
            Priority = 0;
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
            result.Priority = Priority;
            return result;
        }

        #endregion
    }
}
