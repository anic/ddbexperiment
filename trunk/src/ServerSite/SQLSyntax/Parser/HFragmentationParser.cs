using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.Common.Syntax;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class HFragmentationParser:AbstractParser
    {
        HFragmentation result;

        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            result = new HFragmentation();
            //fragment Student horizontally into id<105000, id>=105000 and id<110000, id>=110000
            Regex reg = new Regex(@"fragment\s+(\S+)\s+horizontally\s+into\s+(.*)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Source.TableName = match.Groups[1].ToString();
                string[] conditions = match.Groups[2].ToString().Trim().Split(',');
                ConditionMatcher cm = new ConditionMatcher();
                foreach (string s in conditions)
                {
                    Condition c = cm.MatchCondition(s.Trim());
                    if (c != null)
                        result.FragmentCondition.Add(c);
                    else
                    {
                        error.Description = cm.LastError.Description;
                        return false;
                    }
                }
                return true;
            }
            error.Description = "水平分片格式匹配错误";
            return false;
            
        }

        public override bool FillLocalConsistency()
        {
            return true;
        }
    }
}
