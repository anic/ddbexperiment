using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Object;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    interface ISqlParser
    {
        object LastResult { get; }

        SqlSyntaxError LastError { get; }

        bool Parse(string sql);
    }
}
