using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;


namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class DeletionParser:AbstractParser
    {
        Deletion result;
        
        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            result = new Deletion();
            Regex reg = new Regex(@"(delete)\s+(from)\s+(\S+)\s+(where)\s+(.*)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result.Source.TableName = match.Groups[3].ToString().Trim();
                ConditionMatcher cm = new ConditionMatcher();
                Condition c = cm.MatchCondition(match.Groups[5].ToString().Trim());
                if (c != null)
                {
                    result.Condition = c;
                    return true;
                }
                else
                {
                    error.Description = cm.LastError.Description;
                    return false;
                }
            }

            error.Description = "删除格式不匹配";
            return false;

            //delete from Teacher where id>=290000 and title=2,
        }

        public override bool FillGlobalConsisitency(DistDBMS.Common.Dictionary.GlobalDirectory gdd)
        {
            try
            {
                result.Source = gdd.Schemas[result.Source.TableName].Clone() as TableSchema;
                return true;
            }
            catch {
                LastError.Description = "没有匹配的表";
                return false;
            }
            
        }

       
    }
}
