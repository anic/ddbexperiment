﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.Common.Syntax;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    /// <summary>
    /// 水平分片
    /// </summary>
    class HFragmentation
    {
        /// <summary>
        /// 需要分片的表
        /// </summary>
        public TableSchema Source
        {
            get;
            set;
        }
        

        /// <summary>
        /// 分片的条件
        /// </summary>
        public List<Condition> FragmentCondition { get { return conditions; } }
        List<Condition> conditions;

        public HFragmentation()
        {
            Source = new TableSchema();
            conditions = new List<Condition>();
        }

        public new string ToString()
        {
            //fragment Student horizontally into id<105000, id>=105000 and id<110000, id>=110000
            string result = "fragmen " + Source.ToString() + " horizontally into ";
            for (int i = 0; i < conditions.Count; i++)
            {
                if (i != 0)
                    result += ", ";
                result += conditions[i].ToString();
            }

            return result;
            
        }
    }
}
