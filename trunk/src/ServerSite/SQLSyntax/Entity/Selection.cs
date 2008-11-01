using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Selection
    {
        /// <summary>
        /// 目标样式，Select 后面的样式
        /// </summary>
        public TableScheme Fields
        {
            get { return fields; }
            
        } //Select (A.t1,B.t2)
        TableScheme fields;

        /// <summary>
        /// 源表，就是Select...From后面的表
        /// </summary>
        public List<TableScheme> Sources
        {
            get { return sources; }
        } //From A,B
        List<TableScheme> sources;

        /// <summary>
        /// 条件，Where后面的内容，可嵌套
        /// </summary>
        public Condition Condition { get { return condition; } } //Where ...
        Condition condition;

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public Selection()
        {
            fields = new TableScheme();
            sources = new List<TableScheme>();
            condition = new Condition();
            Content = "";
        }

        public new string ToString()
        {
            string result = "SELECT " + Fields.ToString() + "\nFROM ";
            for (int i = 0; i < Sources.Count; i++)
            {
                if (i != 0)
                    result += ", ";
                result += Sources[i].ToString();
            }
            result += "\nWHERE " + Condition.ToString();
            return result;
           
        }
    }
}
