using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Object;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    abstract class AbstractParser
    {
        public abstract object LastResult { get; }

        public SqlSyntaxError LastError { get { return error; } }
        protected SqlSyntaxError error;

        public abstract bool Parse(string sql);

        public AbstractParser() 
        {
            error = new SqlSyntaxError();
        }
    }
}
