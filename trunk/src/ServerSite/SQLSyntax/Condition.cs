using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    class Condition
    {
        public RelationOperator Operator{get;set;}

        public Condition LeftCondition { get; set; }

        public Condition RightCondition { get; set; }

        public bool IsAtomCondition { get; set; }

        public AtomCondition AtomCondition { get; set; }
    }
}
