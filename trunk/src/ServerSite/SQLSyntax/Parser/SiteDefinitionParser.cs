using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class SiteDefinitionParser:AbstractParser
    {
        SiteDefinition result;

        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            //define site S1 127.0.0.1:2001
            result = new SiteDefinition();
            Regex reg = new Regex(@"define site\s+(\S+)\s+(\S+)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Site.Name = match.Groups[1].ToString();
                string[] address = match.Groups[2].ToString().Trim().Split(':');
                if (address != null && address.Length == 2)
                {
                    result.Site.IP = address[0];
                    try
                    {
                        result.Site.Port = Convert.ToInt32(address[1]);
                    }
                    catch
                    {
                        error.Description = "端口匹配错误";
                        return false;
                    }
                    return true;
                }
                else
                {
                    error.Description = "站点地址匹配错误";
                    return false;
                }
            }
            error.Description = "定义格式匹配错误";
            return false;
        }

        public override bool FillLocalConsistency()
        {
            return true;
        }
    }
}
