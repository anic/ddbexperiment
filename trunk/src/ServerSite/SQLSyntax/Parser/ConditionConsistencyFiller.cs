using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class ConditionConsistencyFiller
    {
        public void FillSingleTableCondition(TableScheme table, Condition c)
        {
            if (c.IsAtomCondition)
            {
                FillSingleTableOperand(table, c.AtomCondition.LeftOperand);
                FillSingleTableOperand(table, c.AtomCondition.RightOperand);
            }
            else
            {
                if (c.LeftCondition != null)
                    FillSingleTableCondition(table, c.LeftCondition);

                if (c.RightCondition != null)
                    FillSingleTableCondition(table, c.RightCondition);
            }
        }

        public void FillSingleTableOperand(TableScheme table, Operand operand)
        {
            if (operand.IsField)
            {
                Field f = table[operand.Field.AttributeName];
                if (f!=null)
                operand.Field = f;
            }
        }
    }
}