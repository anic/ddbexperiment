﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax.Object;
using System.Text.RegularExpressions;
using DistDBMS.Common.Entity;

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
                result.IsAtomCondition = true;
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

        public AtomCondition MatchAtomCondition(string str)
        {
            AtomCondition aCondition = new AtomCondition();
            Regex reg = new Regex(@"(.*)\s*(>|>=|<|<=|=|<>)\s*(.*)");
            Match match = reg.Match(str);
            if (match.Success)
            {
                Operand operand1 = MatchOperand(match.Groups[1].ToString().Trim());
                if (operand1 != null)
                    aCondition.LeftOperand = operand1;
                else
                    return null;

                operand1 = MatchOperand(match.Groups[3].ToString().Trim());
                if (operand1 != null)
                    aCondition.RightOperand = operand1;
                else
                    return null;

                switch (match.Groups[2].ToString())
                { 
                    case ">":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.Greater;
                        break;
                    case ">=":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.GreaterOrEqual;
                        break;
                    case "<":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.Less;
                        break;
                    case "<=":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.LessOrEqual;
                        break;
                    case "=":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.Equal;
                        break;
                    case "<>":
                        aCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.NotEqual;
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
                result.ValueType = DistDBMS.Common.Entity.AttributeType.Int;
                result.Value = str;
                return result;
            }

            reg = new Regex(@"\'(.*)\'");
            match = reg.Match(str);
            if (match.Success)
            { 
                //字符串
                result.IsValue = true;
                result.ValueType = DistDBMS.Common.Entity.AttributeType.String;
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