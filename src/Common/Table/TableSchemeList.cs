using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;

namespace DistDBMS.Common.Dictionary
{
    [Serializable]
    public class TableSchemaList:List<TableSchema>
    {
        /// <summary>
        /// 优化使用，记录联通集的join条件
        /// </summary>
        public List<AtomCondition> JoinPredication { get; set; }
        
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

        public TableSchemaList()
        {
            JoinPredication = new List<AtomCondition>();
        }
    }
}
