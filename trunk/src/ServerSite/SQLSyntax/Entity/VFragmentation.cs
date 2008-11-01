using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class VFragmentation
    {
        TableScheme source;
        /// <summary>
        /// 需要分片的表
        /// </summary>
        public TableScheme Source { get { return source; } }

        List<TableScheme> schemes;
        List<TableScheme> Schemes { get { return schemes; } }

        public VFragmentation()
        {
            source = new TableScheme();
            schemes = new List<TableScheme>();
        }

        public new string ToString() 
        {
            //fragment Course vertically into (id, name), (id, location, credit_hour, teacher_id)
            string result = "fragment " + source.ToString() + " vertically into ";
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
