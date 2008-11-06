using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.ControlSite.SQLSyntax.Parser;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax
{
    class GlobalConsitencyFiller
    {
        public bool FillSelection(GlobalDirectory gdd, Selection s)
        {

            for (int i = 0; i < s.Sources.Count; i++)
            {
                TableSchema logicTable = gdd.Schemas[s.Sources[i].TableName];
                if (logicTable != null)
                    s.Sources[i] = logicTable;
                else
                    return false;
            }

            //TODO:现在是默认只有一个from的表，条件中才可以不写表名称
            if (s.Sources.Count == 1 && s.Condition != null)
            {
                ConditionConsistencyFiller filler = new ConditionConsistencyFiller();
                TableSchemaList list = new TableSchemaList();
                list.AddRange(s.Sources);
                filler.FillCondition(list, s.Condition);
            }

            //TODO:现在是默认只有一个from的表，select后才可以不写表名
            if (s.Sources.Count == 1)
            {
                if (!s.Fields.IsAllFields)
                {
                    for (int i = 0; i < s.Fields.Fields.Count; i++)
                    {
                        Field f = s.Sources[0][s.Fields.Fields[i].AttributeName];
                        if (f != null)
                            s.Fields.Fields[i] = f; //替换属性
                    }
                }
                else //select *
                {
                    s.Fields.Fields.AddRange(s.Sources[0].Fields);
                    s.Fields.IsAllFields = false;
                }
            }

            return true;
        }

    }
}
