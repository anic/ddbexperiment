using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    class AtomCondition
    {
        public LogicOperator Operator{get;set;}

        public Object LeftOperand { get; set; } //还是说用接口

        public Object RightOperand { get; set; }
    }
}
