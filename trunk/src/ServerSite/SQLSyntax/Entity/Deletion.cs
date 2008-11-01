using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Deletion
    {
        /// <summary>
        /// 删除的源表
        /// </summary>
        public TableScheme Source { get { return source; } }
        TableScheme source;

        /// <summary>
        /// 条件
        /// </summary>
        public Condition Condition { get { return condition; } }
        Condition condition;

        public string Content { get; set; }

        public Deletion()
        {
            source = new TableScheme();
            condition = new Condition();

            Content = "";
        }

        public new string ToString()
        {
            //delete from Teacher where title=1
            return "delete from " + source.TableName + " where " + condition.ToString();
        }
    }
}
