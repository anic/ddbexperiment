using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common
{
    public enum AttributeType
    {
        Unknown,
        Undefined,
        Int,
        Boolean,
        Double,
        String,
        DateTime
    }

    public enum RelationOperator
    {
        And,
        Or
    }

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
