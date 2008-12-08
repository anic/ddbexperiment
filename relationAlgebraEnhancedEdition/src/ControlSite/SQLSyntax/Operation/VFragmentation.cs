using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    class VFragmentation
    {
        
        /// <summary>
        /// 需要分片的表
        /// </summary>
        public TableSchema Source { get; set; }

        
        /// <summary>
        /// 分成的表的样式
        /// </summary>
        public List<TableSchema> Schemas { get { return schemas; } }
        List<TableSchema> schemas;


        public VFragmentation()
        {
            Source = new TableSchema();
            schemas = new List<TableSchema>();
        }

        public new string ToString() 
        {
            //fragment Course vertically into (id, name), (id, location, credit_hour, teacher_id)
            string result = "fragment " + Source.ToString() + " vertically into ";
            for (int i = 0; i < schemas.Count; i++)
            {
                if (i != 0)
                    result += ", ";

                result += schemas[i].ToString();
            }
            return result;
        }
    }
}
