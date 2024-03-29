﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    class ParserSwitcher
    {
        public ParserSwitcher()
        {
            error = "";
            result = null;
        }

        public object LastResult { get { return result; } }
        object result;

        public string LastError { get { return error; } }
        string error;

        public bool Parse(string sql,GlobalDirectory gdd)
        {
            sql = sql.Trim();
            AbstractParser parser = null;

            //获得对应的Parser
            if (sql.IndexOf("select", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new SelectionParser();
            else if (sql.IndexOf("allocate", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new AllocationParser();
            else if (sql.IndexOf("insert", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new InsertionParser();
            else if (sql.IndexOf("delete", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new DeletionParser();
            else if (sql.IndexOf("define", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new SiteDefinitionParser();
            else if (sql.IndexOf("create", StringComparison.CurrentCultureIgnoreCase) == 0)
                parser = new TableCreationParser();
            else if (sql.IndexOf("fragment", StringComparison.CurrentCultureIgnoreCase) == 0)
            { 
                if (sql.IndexOf("horizontally", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    parser = new HFragmentationParser();
                else
                    parser = new VFragmentationParser();
            }
            //解析
            bool parseResult = false;
            if (parser != null)
            {
                parseResult = parser.Parse(sql);

                if (parseResult && parser != null)
                {
                    //本地语意检查
                    parseResult |= parser.FillLocalConsistency();

                    //全局语意检查
                    if (parseResult && gdd != null)
                        parseResult |= parser.FillGlobalConsisitency(gdd);

                    if (parseResult)
                        result = parser.LastResult;
                    else
                        error = parser.LastError.Description;
                }
                else
                    error = parser.LastError.Description;

                return parseResult;
            }
            else
            {
                error = "不识别的SQL语句";
                return false;
            
            }
            

        }

    }
}
