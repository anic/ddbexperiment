using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
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
            Regex reg = new Regex(@"define\s*site\s+(\S+)\s+(\S+)\s+(\S+)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Site.Name = match.Groups[1].ToString();
                result.Site.IP = match.Groups[2].ToString();
                
                try
                {
                    result.Site.Port = Convert.ToInt32(match.Groups[3].ToString());
                }
                catch
                {
                    error.Description = "端口匹配错误";
                    return false;
                }
                return true;
            
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
