using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class TableCreationParser:AbstractParser
    {
        TableCreation result;

        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            result = new TableCreation();
            //create table Student (id int key, name char(25), sex char(1), age int, degree int)
            Regex reg = new Regex(@"create table\s+(\S+)\s*\((.*)\)\s*",RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Target.TableName = match.Groups[1].ToString().Trim();
                string[] attribute = match.Groups[2].ToString().Split(',');
                foreach (string s in attribute)
                {
                    Field f = MatchField(s.Trim());
                    if (f != null)
                        result.Target.Fields.Add(f);
                    else
                        return false;
                }
                return true;
            }
            error.Description = "创建格式匹配错误";
            return false;
        }

        private Field MatchField(string strField)
        {
            Field f = new Field();
            string[] items = strField.Split(' ');
            if (items != null)
            {
                //匹配关键字ID: id int key
                if (items.Length == 3 && items[2].IndexOf("key", StringComparison.CurrentCultureIgnoreCase)==0)
                {
                    f.IsPrimaryKey = true;
                    f.AttributeName = items[0];
                    if (MatchType(items[1], ref f))
                        return f;
                    else
                        return null;
                }
                else if (items.Length == 2) // name char(25)
                {
                    f.IsPrimaryKey = false;
                    f.AttributeName = items[0];
                    if (MatchType(items[1], ref f))
                        return f;
                    else
                        return null;
                }
            }
            error.Description = "属性域匹配错误";
            return null;
        }

        private bool MatchType(string strType,ref Field f)
        {
            if (strType.IndexOf("int", StringComparison.CurrentCultureIgnoreCase) == 0) //匹配Int
            {
                f.AttributeType = DistDBMS.Common.AttributeType.Int;
                return true;
            }
            else if (strType.IndexOf("char", StringComparison.CurrentCultureIgnoreCase) == 0) //匹配char
            {
                //嵌入的规则，char转化成String
                f.AttributeType = DistDBMS.Common.AttributeType.String;
                
                Regex reg = new Regex(@"char\s*\(([0-9]+)\)",RegexOptions.IgnoreCase);
                Match match = reg.Match(strType);
                if (match.Success)
                {
                    try
                    {
                        f.Addition = Convert.ToInt32(match.Groups[1].ToString());
                        return true;
                    }
                    catch
                    {
                        error.Description = "数字匹配错误";
                        return false;
                    }
                }
                return false;
            }
            error.Description = "未知类型匹配";
            return false;
        }

        public override bool FillLocalConsistency()
        {
            result.Target.IsDbTable = true;
            result.Target.IsAllFields = true;
            foreach (Field f in result.Target.Fields)
                f.TableName = result.Target.TableName;

            return true;
        }
    }
}
