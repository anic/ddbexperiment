using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    enum RelationOperator
    {
        And,
        Or
    }

    enum LogicOperator
    {
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual
    }

    enum AttributeType
    { 
        Unkown,
        Undefined,
        Int,
        Boolean,
        Double,
        String
    }

}
