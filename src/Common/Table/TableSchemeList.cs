using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;

namespace DistDBMS.Common.Dictionary
{
    public class TableSchemaList:List<TableSchema>
    {
        public TableSchema this[string key]
        {
            get
            {
                foreach (TableSchema t in this)
                    if (t.TableName == key || t.NickName == key)
                        return t;

                return null;
            }
        }
    }
}
