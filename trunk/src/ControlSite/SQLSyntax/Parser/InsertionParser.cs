using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class InsertionParser:AbstractParser
    {
        
        Insertion result;

        public InsertionParser()
        {
        }

       
        private Tuple MatchTuple(string str)
        {
            Regex reg = new Regex(@"^\((.*)\)$", RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            if (match.Success)
            {
                string content = match.Groups[1].ToString();
                string[] values = content.Split(',');
                Tuple result = new Tuple();
                foreach (string s in values)
                {
                    //todo:如果是字符串，是否需要摘除‘’，“”
                    result.Data.Add(s.Trim());
                }
                return result;
            }
            return null;
        }

        public override bool Parse(string sql)
        {
            result = new Insertion();
            //insert into Student values (190001, 'xiao ming', 'M', 20, 1)
            Regex reg = new Regex(@"(insert)\s+(into)\s+(\S+)\s+(values)\s+(.*)",RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Target.TableName = match.Groups[3].ToString();

                Tuple t = MatchTuple(match.Groups[5].ToString().Trim());
                if (t != null)
                {
                    result.Data = t;
                    return true;
                }
                else
                {
                    error.Description = "插入值匹配错误";
                    return false;
                }
            }

            error.Description = "Insert语法不正确";
            return false;
        }


        public override object LastResult
        {
            get { return result; }
        }

        public override bool FillLocalConsistency()
        {
            return true;
        }
    }
}
