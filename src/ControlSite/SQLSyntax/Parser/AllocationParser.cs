﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using System.Text.RegularExpressions;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class AllocationParser:AbstractParser
    {
        
        Allocation result;

        public AllocationParser()
        {
        }

        public override object LastResult
        {
            get { return result; }
        }

        public override bool Parse(string sql)
        {
            //allocate Student.2 to S2
            Regex reg = new Regex(@"(allocate)\s*(\S*)\s*(to)\s*(\S*)\s*", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (match.Success)
            {
                result = new Allocation();
                Site site = new Site();
                site.Name = match.Groups[4].ToString();

                result.Site = site;
                result.Table.TableName = match.Groups[2].ToString();

                return true;
            }

            error.Description = "站点分配格式不匹配";
            return false;

        }

    }
}
