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
                    string s1 = s.Trim();
                    //todo:如果是字符串，是否需要摘除‘’，“”
                    if (s1.StartsWith("\'") && s1.EndsWith("\'"))
                        s1 = s1.Trim(new char[] { '\'' });
                    
                    result.Data.Add(s1);
                }
                return result;
            }
            return null;
        }

        public override bool Parse(string sql)
        {
            result = new Insertion();
            //insert into Student values (190001, 'xiao ming', 'M', 20, 1)
            //insert into Customer(id, name, gender, rank) values(100010, 'Xiaoming', 'M',1)
            //TODO:新的格式有变化
            /*Regex reg = new Regex(@"(insert)\s+(into)\s+(\S+)\s+(values)\s+(.*)",RegexOptions.IgnoreCase);
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
            }*/
            try
            {
                int index_afterInto = sql.IndexOf("into", StringComparison.CurrentCultureIgnoreCase) + 4;
                int index_value = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase);
                if (index_afterInto >= 0
                    && index_value >= 0
                    && index_afterInto <= sql.Length - 1
                    && index_value <= sql.Length - 1)
                {
                    string strTable = sql.Substring(index_afterInto, index_value - index_afterInto).Trim();
                    string strTableName = strTable.Substring(0, strTable.IndexOf("("));
                    result.Target = new TableSchema();
                    result.Target.TableName = strTableName;
                    string strTuple = sql.Substring(index_value + 6);

                    Tuple t = MatchTuple(strTuple);
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
            }
            catch { }
            error.Description = "Insert语法不正确";
            return false;
        }


        public override object LastResult
        {
            get { return result; }
        }

        public override bool FillGlobalConsisitency(DistDBMS.Common.Dictionary.GlobalDirectory gdd)
        {
            result.Target = gdd.Schemas[result.Target.TableName] as TableSchema; //这里只能临时填充表
            return true;
        }
    }
}
