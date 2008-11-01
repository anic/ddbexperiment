using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Operation;
using DistDBMS.ServerSite.SQLSyntax.Object;
using System.Text.RegularExpressions;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class SelectionParser:ISqlParser
    {
        Selection result;

        SqlSyntaxError error;
        TableMatcher matcher;
        public SelectionParser()
        {
            error = new SqlSyntaxError();
            matcher = new TableMatcher();
        }


        #region ISqlParser Members

        public object LastResult
        {
            get { return result; }
        }

        public SqlSyntaxError LastError
        {
            get { return error; }
        }

        public bool Parse(string sql)
        {
            //string s1 = "select Course.name, Course.credit_hour, Teacher.name from b where c";
            string condition = "";
            string source ="";
            Regex reg = new Regex(@"(select)\s*(.*)\s*(from)\s*(.*)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success && match.Groups.Count == 5)
            {
                result = new Selection();

                //匹配目标的表样式
                TableScheme t = matcher.MatchTableScheme(match.Groups[2].ToString().Trim());
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
                List<TableScheme> sources = matcher.MatchMoreTableScheme(source.Trim());
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

        #endregion
    }
}
