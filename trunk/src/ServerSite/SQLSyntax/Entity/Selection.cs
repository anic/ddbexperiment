using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Selection
    {
        TableScheme fields;
        public TableScheme Fields { 
            get
            {
                return fields;
            }
        } //Select (A.t1,B.t2)

        List<TableScheme> sources;
        public ICollection<TableScheme> Sources
        {
            get {
                return sources;
            }
        } //From A,B

        public Condition Condition { get; set; } //Where ...

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public Selection()
        {
            fields = new TableScheme();
            sources = new List<TableScheme>();
            Condition = new Condition();
            Content = "";
        }

    }
}
