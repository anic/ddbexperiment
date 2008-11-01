using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.GDD;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Allocation
    {
        /// <summary>
        /// 构造一个站点对象
        /// </summary>
        public Site Site { get { return site; } }
        Site site;

        public string Content { get; set; }

        public TableScheme Table { get { return table; } }
        TableScheme table;

        public Allocation()
        {
            site = new Site();
            table = new TableScheme();
            Content = "";
        }

        public new string ToString()
        { 
            //allocate Student.2 to S2
            return "allocate " + table.TableName + " to " + site.Name;
        }
    }
}
