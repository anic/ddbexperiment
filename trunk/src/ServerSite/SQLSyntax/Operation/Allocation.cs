﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.GDD;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Operation
{
    class Allocation
    {
        /// <summary>
        /// 构造一个站点对象
        /// </summary>
        public Site Site { get; set; }
        
        public string Content { get; set; }

        public TableScheme Table { get; set; }
        
        public Allocation()
        {
            Site = new Site();
            Table = new TableScheme();
            Content = "";
        }

        public new string ToString()
        { 
            //allocate Student.2 to S2
            return "allocate " + Table.TableName + " to " + Site.Name;
        }
    }
}