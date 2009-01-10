using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common
{
    /// <summary>
    /// 属性类型
    /// </summary>
    public enum AttributeType
    {
        Unknown,
        Undefined,
        Int,
        Boolean,
        Double,
        String,
        DateTime,
        BLOB
    }

    /// <summary>
    /// 关系符
    /// </summary>
    public enum RelationOperator
    {
        And,
        Or
    }

    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum LogicOperator
    {
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual
    }
}
