using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.Common.Syntax;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    class Deletion
    {
        /// <summary>
        /// 删除的源表
        /// </summary>
        public TableSchema Source { get; set; }
        

        /// <summary>
        /// 条件
        /// </summary>
        public Condition Condition { get; set; }
        

        public string Content { get; set; }

        public Deletion()
        {
            Source = new TableSchema();
            Condition = new Condition();

            Content = "";
        }

        public new string ToString()
        {
            //delete from Teacher where title=1
            return "delete from " + Source.TableName + " where " + Condition.ToString();
        }
    }
}
