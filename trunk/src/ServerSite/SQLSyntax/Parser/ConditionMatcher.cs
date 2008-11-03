using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Object;
using System.Text.RegularExpressions;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;
using DistDBMS.Common;

namespace DistDBMS.ServerSite.SQLSyntax.Parser
{
    class ConditionMatcher
    {
        public ConditionMatcher()
        {
            error = new SqlSyntaxError();
        }
        
        SqlSyntaxError error;
        public SqlSyntaxError LastError { get { return error; } }

        public Condition MatchCondition(string str)
        {
            Condition result = new Condition();
            if (IsAtomCondition(str))
            {
                AtomCondition ac = MatchAtomCondition(str);
                result.AtomCondition = ac;
                return result;
            }
            
            Regex reg = new Regex(@"(.*)\s*(and|or)\s*(.*)", RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            if (match.Success)
            {
                string left = match.Groups[1].ToString().Trim();
                Condition c = MatchCondition(left);
                if (c != null)
                    result.LeftCondition = c;
                else
                {
                    error.Description = "匹配出错";
                    return null;
                }

                string right = match.Groups[3].ToString().Trim();
                c = MatchCondition(right);
                if (c != null)
                    result.RightCondition = c;
                else
                {
                    error.Description = "匹配出错";
                    return null;
                }

                return result;
            }

            error.Description = "匹配出错";
            return null;


        }

        private bool IsAtomCondition(string str)
        {
            return !(str.IndexOf("And", StringComparison.CurrentCultureIgnoreCase) >= 0
                || str.IndexOf("Or", StringComparison.CurrentCultureIgnoreCase) >= 0);

        }

        /// <summary>
        /// 匹配原子条件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public AtomCondition MatchAtomCondition(string str)
        {
            string op = "";
            string left ="";
            string right = "";

            AtomCondition aCondition = new AtomCondition();
            Regex reg = new Regex(@"(.*)\s*(<>|>=|<=)\s*(.*)");
            Match match = reg.Match(str);
            
            bool result = false;
            result |= match.Success;
            if (match.Success)
            {
                op = match.Groups[2].ToString();
                left = match.Groups[1].ToString().Trim();
                right = match.Groups[3].ToString().Trim();
            }
            else
            {
                reg = new Regex(@"(.*)\s*(>|<|=)\s*(.*)");
                match = reg.Match(str);
                result |= match.Success;
                if (match.Success)
                {
                    op = match.Groups[2].ToString();
                    left = match.Groups[1].ToString().Trim();
                    right = match.Groups[3].ToString().Trim();
                }
                else
                {
                    error.Description = "比较符不匹配";
                    return null;
                }
            }


            if (result)
            {
                Operand operand1 = MatchOperand(left.Trim());
                if (operand1 != null)
                    aCondition.LeftOperand = operand1;
                else
                    return null;

                operand1 = MatchOperand(right.Trim());
                if (operand1 != null)
                    aCondition.RightOperand = operand1;
                else
                    return null;

                switch (op.Trim())
                { 
                    case ">":
                        aCondition.Operator = LogicOperator.Greater;
                        break;
                    case ">=":
                        aCondition.Operator = LogicOperator.GreaterOrEqual;
                        break;
                    case "<":
                        aCondition.Operator = LogicOperator.Less;
                        break;
                    case "<=":
                        aCondition.Operator = LogicOperator.LessOrEqual;
                        break;
                    case "=":
                        aCondition.Operator = LogicOperator.Equal;
                        break;
                    case "<>":
                        aCondition.Operator = LogicOperator.NotEqual;
                        break;
                }

                return aCondition;
            }

            return null;
        }

        public Operand MatchOperand(string str)
        {
            str = str.Trim();
            Operand result = new Operand();
            Regex reg = new Regex(@"^[0-9]+$");
            Match match = reg.Match(str);
            if (match.Success)
            {
                //数字
                result.IsValue = true;
                result.ValueType = AttributeType.Int;
                result.Value = str;
                return result;
            }

            reg = new Regex(@"^\'(.*)\'$");
            match = reg.Match(str);
            if (match.Success)
            { 
                //字符串
                result.IsValue = true;
                result.ValueType = AttributeType.String;
                result.Value = match.Groups[1].ToString();
                return result;
            }

            //当做属性
            TableMatcher tm = new TableMatcher();
            Field f = tm.MatchField(str.Trim());
            if (f != null)
            {
                result.IsValue = false;
                result.Field = f;
                return result;
            }

            error.Description = "操作数匹配错误";
            return null;
            

            
        }


    }
}
