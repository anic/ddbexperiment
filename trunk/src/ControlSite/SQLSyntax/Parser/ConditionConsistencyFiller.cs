using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class ConditionConsistencyFiller
    {
        public void FillCondition(TableScheme table, Condition c)
        {

            TableSchemeList list = new TableSchemeList();
            list.Add(table);

            FillCondition(list, c);
        }


        public void FillCondition(TableSchemeList tables, Condition c)
        {
            if (c.IsAtomCondition)
            {
                FillOperand(tables, c.AtomCondition.LeftOperand);
                FillOperand(tables, c.AtomCondition.RightOperand);
            }
            else
            {
                if (c.LeftCondition != null)
                    FillCondition(tables, c.LeftCondition);

                if (c.RightCondition != null)
                    FillCondition(tables, c.RightCondition);
            }
        }

        public void FillOperand(TableSchemeList tables,Operand operand)
        {
            if (operand.IsField)
            {
                foreach (TableScheme table in tables)
                {
                    Field f = table[operand.Field.AttributeName];
                    if (f != null)
                    {
                        operand.Field = f;
                        return;
                    }

                }
            }
        }
    }
}