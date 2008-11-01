using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Operation
{
    class VFragmentation
    {
        TableScheme source;
        /// <summary>
        /// 需要分片的表
        /// </summary>
        public TableScheme Source { get; set; }

        List<TableScheme> schemes;
        List<TableScheme> Schemes { get { return schemes; } }

        public VFragmentation()
        {
            Source = new TableScheme();
            schemes = new List<TableScheme>();
        }

        public new string ToString() 
        {
            //fragment Course vertically into (id, name), (id, location, credit_hour, teacher_id)
            string result = "fragment " + Source.ToString() + " vertically into ";
            for (int i = 0; i < schemes.Count; i++)
            {
                if (i != 0)
                    result += ", ";

                result += schemes[i].ToString();
            }
            return result;
        }
    }
}
