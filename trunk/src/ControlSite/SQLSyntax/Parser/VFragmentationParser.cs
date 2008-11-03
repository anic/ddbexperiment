using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class VFragmentationParser:AbstractParser
    {
        VFragmentation result;

        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            result = new VFragmentation();
            //fragment Course vertically into (id, name), (id, location, credit_hour, teacher_id)
            Regex reg = new Regex(@"fragment\s+(\S+)\s+vertically\s+into\s+(.*)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Source.TableName = match.Groups[1].ToString();
                string schemes = match.Groups[2].ToString().Trim();
                int left, right;
                left = schemes.IndexOf('(');
                right = schemes.IndexOf(')');
                if ((left >= 0 && right < 0) || (left < 0 && right >= 0))
                {
                    error.Description = "括号不匹配";
                    return false;
                }
                TableMatcher tm = new TableMatcher();
                while (left >= 0 && right >= 0)
                {
                    string scheme = schemes.Substring(left + 1, right - left - 1);
                    TableScheme t = tm.MatchTableScheme(scheme);
                    if (t != null)
                        result.Schemes.Add(t);
                    else
                    {
                        error.Description = tm.LastError.Description;
                        return false;
                    }

                    schemes = schemes.Substring(right + 1);
                    left = schemes.IndexOf('(');
                    right = schemes.IndexOf(')');
                    if ((left>=0 && right <0)||(left< 0 && right >=0))
                    {
                        error.Description = "括号不匹配";
                        return false;
                    }
                }
                return true;
            }
            error.Description = "垂直分片格式匹配错误";
            return false;
        }

        public override bool FillLocalConsistency()
        {
            return true;
        }
    }
}
