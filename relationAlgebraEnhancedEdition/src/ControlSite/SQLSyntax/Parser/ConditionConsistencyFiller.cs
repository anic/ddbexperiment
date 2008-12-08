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
        public void FillCondition(TableSchema table, Condition c)
        {

            TableSchemaList list = new TableSchemaList();
            list.Add(table);

            FillCondition(list, c);
        }


        public void FillCondition(TableSchemaList tables, Condition c)
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

        public void FillOperand(TableSchemaList tables,Operand operand)
        {
            if (operand.IsField)
            {
                foreach (TableSchema table in tables)
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