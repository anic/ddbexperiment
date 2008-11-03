using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;

namespace DistDBMS.ServerSite.Finder
{
    class FragmentFinder
    {
        GlobalDataDictionary gdd;
        public FragmentFinder(GlobalDataDictionary gdd)
        {
            this.gdd = gdd;
        }

        public FragmentList GetMatchFragments(Condition condition)
        {
            FragmentList result = new FragmentList();
            TableSchemeList tables = new TableSchemeList();
            //TODO:未完成
            FindTableByCondition(condition, tables);


            //Fragment beginFragment = gdd.Fragments.GetFragmentByName();
            return result;
        }

        public FragmentList GetMatchFragments(TableScheme table)
        {
            return null;
        }

        public bool MatchFragment(AtomCondition condition, Fragment f)
        {
            return false;
        }

        public TableSchemeList GetRelatedTable(Condition condition)
        {
            TableSchemeList result = new TableSchemeList();
            FindTableByCondition(condition, result);
            return result;
        }

        private void FindTableByCondition(Condition condition, TableSchemeList list)
        {
            if (condition.IsAtomCondition)
            {
                
                TableScheme t;
                TableScheme t2;

                t = FindTableByOperand(condition.AtomCondition.LeftOperand);
                if (t != null)
                {
                    t2 = list[t.TableName];
                    if (t2 == null)
                        list.Add(t);
                    else
                        MerageTable(t2, t);
                }
                

                t = FindTableByOperand(condition.AtomCondition.RightOperand);
                if (t != null)
                {
                    t2 = list[t.TableName];
                    if (t2 == null)
                        list.Add(t);
                    else
                        MerageTable(t2, t);
                }
            }
            else
            {
                if (condition.LeftCondition != null)
                    FindTableByCondition(condition.LeftCondition, list);

                if (condition.RightCondition != null)
                    FindTableByCondition(condition.RightCondition, list);
            }
        }

        /// <summary>
        /// 将两个表格Merage
        /// </summary>
        /// <param name="t1">表格1</param>
        /// <param name="t2">表格2</param>
        ///<returns></returns>
        ///<remarks>结果将会在表格1中返回</remarks>
        private bool MerageTable(TableScheme t1,TableScheme t2)
        { 
            foreach(Field f2 in t2.Fields)
            {
                Field f1 = t1[f2.AttributeName];
                if (f1 == null)
                    t1.Fields.Add(f2);
            }
            return true;
        }

        public TableScheme FindTableByOperand(Operand operand)
        {
            if (operand.IsField)
                return FindTableByField(operand.Field);
            else
                return null;
        }

        public TableScheme FindTableByField(Field f)
        {
            TableScheme logicTable = gdd.Schemes[f.TableName];
            if (logicTable != null)
            {
                TableScheme tempTable = new TableScheme();
                tempTable.TableName = f.TableName;
                Field field = logicTable[f.AttributeName];
                if (field != null)
                    tempTable.Fields.Add(field);
                return tempTable;
            }
            else
                return null;
        }

        public SiteList GetMatchFragmentSites(Condition condition)
        {
            SiteList result = new SiteList();
            FragmentList fragmentList = GetMatchFragments(condition);
            if (fragmentList != null)
            {
                foreach (Fragment f in fragmentList)
                {
                    if (f.Site != null && result[f.Site.Name] == null)
                        result.Add(f.Site);
                }
            }
            return result;
        }
    }
}
