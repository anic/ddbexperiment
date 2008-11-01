using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Insertion
    {
        TableScheme target;
        /// <summary>
        /// 需要插入的表格
        /// </summary>
        public TableScheme Target { get { return target; } }

        Tuple data;
        /// <summary>
        /// 插入的元组
        /// </summary>
        public Tuple Data { get { return data; } }
        

        public Insertion()
        {
            target = new TableScheme();
            data = new Tuple();
        }

        public new string ToString()
        {
            //insert into Student values (190001, 'xiao ming', 'M', 20, 1)
            return "insert into " + Target.ToString() + " values " + data.ToString();
        }
    }
}
