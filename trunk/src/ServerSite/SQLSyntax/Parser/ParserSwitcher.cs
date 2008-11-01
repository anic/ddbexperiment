using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DistDBMS.ServerSite.SQLSyntax.Operation;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class ParserSwitcher
    {
        public object LastResult { get { return result; } }
        object result;

        public string LastError { get { return error; } }
        string error;

        public bool Parse(string sql)
        {
            sql = sql.Trim();
            Regex reg;
            Match match;
            ISqlParser parser = null;
            bool parseResult = false;
            reg = new Regex("select", RegexOptions.IgnoreCase);
            match = reg.Match(sql);
            if (match.Success && match.Index == 0)
                parser = new SelectionParser();

            if (parser != null)
                parseResult = parser.Parse(sql);

            if (parseResult && parser!=null)
                result = parser.LastResult;
            else
                error = parser.LastError.Description;

            return parseResult;

        }

    }
}
