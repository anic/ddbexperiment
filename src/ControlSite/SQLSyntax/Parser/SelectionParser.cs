using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.ControlSite.SQLSyntax.Object;
using System.Text.RegularExpressions;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class SelectionParser:AbstractParser
    {
        Selection result;
        
        TableMatcher matcher;
        public SelectionParser()
        {
            matcher = new TableMatcher();
        }


        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            //string s1 = "select Course.name, Course.credit_hour, Teacher.name from b where c";
            string condition = "";
            string source ="";
            Regex reg = new Regex(@"(select)\s*(.*)\s*(from)\s*(.*)\s*", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result = new Selection();

                //匹配目标的表样式
                TableSchema t = matcher.MatchTableSchema(match.Groups[2].ToString().Trim());
                if (t != null)
                    result.Fields = t;
                else
                {
                    error.Description = matcher.LastError.Description;
                    return false;
                }


                //检查是否有条件
                Regex reg2 = new Regex(@"(.*)(where)(.*)");
                Match match2 = reg2.Match(match.Groups[4].ToString().Trim());
                if (match2.Success && match2.Groups.Count == 4)
                {
                    //有条件
                    source = match2.Groups[1].ToString();
                    condition = match2.Groups[3].ToString().Trim();

                    ConditionMatcher cm = new ConditionMatcher();
                    Condition c = cm.MatchCondition(condition);
                    if (c != null)
                        result.Condition = c;
                    else
                    {
                        error.Description = cm.LastError.Description;
                        return false;
                    }

                }
                else
                {
                    //无条件
                    source = match.Groups[4].ToString();
                }


                //匹配源表格
                List<TableSchema> sources = matcher.MatchMoreTableSchema(source.Trim());
                if (source != null)
                    result.Sources.AddRange(sources);
                else
                {
                    error.Description = matcher.LastError.Description;
                    return false;
                }

            }


            return true;
        }

    }
}
