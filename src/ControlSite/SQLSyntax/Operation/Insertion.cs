﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    class Insertion
    {
        
        /// <summary>
        /// 需要插入的表格
        /// </summary>
        public TableSchema Target { get; set; }

        
        /// <summary>
        /// 插入的元组
        /// </summary>
        public Tuple Data { get; set; }
        

        public Insertion()
        {
            Target = new TableSchema();
            Data = new Tuple();
        }

        public new string ToString()
        {
            //insert into Student values (190001, 'xiao ming', 'M', 20, 1)
            return "insert into " + Target.ToString() + " values " + Data.ToString();
        }
    }
}
