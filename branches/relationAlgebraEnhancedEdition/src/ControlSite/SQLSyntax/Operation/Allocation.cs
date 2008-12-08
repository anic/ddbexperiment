using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    class Allocation
    {
        /// <summary>
        /// 构造一个站点对象
        /// </summary>
        public Site Site { get; set; }
        
        

        public TableSchema Table { get; set; }
        
        public Allocation()
        {
            Site = new Site();
            Table = new TableSchema();
            //Content = "";
        }

        public new string ToString()
        { 
            //allocate Student.2 to S2
            return "allocate " + Table.TableName + " to " + Site.Name;
        }
    }
}
