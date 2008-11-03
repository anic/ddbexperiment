using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.Common.Table;
using System.Text.RegularExpressions;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class TableMatcher
    {
        public TableMatcher()
        {
            error = new SqlSyntaxError();
        }
        
        SqlSyntaxError error;
        public SqlSyntaxError LastError { get { return error; } }

        public TableScheme MatchOneTable(string str)
        {
            //Student S

            TableScheme result = new TableScheme();
            result.IsAllFields = true;

            string[] items = str.Split(' ');
            if (items != null)
            {
                if (items.Length == 2)
                {
                    result.TableName = items[0];
                    result.NickName = items[1];
                    return result;
                }
                else if (items.Length >= 1)
                {
                    result.TableName = items[0];
                    return result;
                }
                
            }
            error.Description = "表格式错误";
            return null;
            

        }

        public List<TableScheme> MatchMoreTableScheme(string str)
        {
            List<TableScheme> result = new List<TableScheme>();
            // Student, Course ,...
            string[] sources = str.Split(',');
            foreach (string s in sources)
            {
                TableScheme t = MatchOneTable(s.Trim());
                if (t != null)
                    result.Add(t);
                else
                {
                    error.Description = "多表格式错误";
                    return null;
                }
            }
            return result;
        }

        

        /// <summary>
        /// 匹配*或者id,name,...等表样式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public TableScheme MatchTableScheme(string str)
        {
            
            Regex reg;
            Match match;
            TableScheme result = new TableScheme();
            reg = new Regex(@"\*", RegexOptions.IgnoreCase);
            match = reg.Match(str);
            if (match.Success)
            {
                result.IsAllFields = true;
                return result;
            }
            else
            {
                string[] fields = str.Split(',');
                foreach (string s in fields)
                {
                    Field f = MatchField(s.Trim());
                    if (f != null)
                        result.Fields.Add(f);
                    else
                    {
                        error.Description = "无法匹配表内属性";
                        return null;
                    }
                }
                return result;
            }
        }

        public Field MatchField(string str)
        {
            string[] items = str.Split('.');
            if (items != null)
            {
                if (items.Length == 2)
                {
                    Field f = new Field();
                    f.AttributeName = items[1];
                    f.TableName = items[0];
                    return f;
                }
                else if (items.Length == 1)
                {
                    Field f = new Field();
                    f.AttributeName = items[0];
                    return f;
                }
            }

            return null;
            
        }

    }
}
