using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.SQLSyntax.Parser;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Execution;

namespace DistDBMS.ControlSite
{
    public class GDDCreator
    {
        List<SiteDefinition> siteDefList = new List<SiteDefinition>();
        List<TableCreation> tableCreationList = new List<TableCreation>();
        List<Allocation> allocationList = new List<Allocation>();
        List<HFragmentation> hFragList = new List<HFragmentation>();
        List<VFragmentation> vFragList = new List<VFragmentation>();

        ParserSwitcher ps = new ParserSwitcher();

        GlobalDirectory gdd;
        Queue<object> fragmentQueue = new Queue<object>();

        public void InitCreatioin()
        {
            
            gdd = null;

            siteDefList.Clear();
            tableCreationList.Clear();
            allocationList.Clear();
            hFragList.Clear();
            vFragList.Clear();
            fragmentQueue.Clear();
        }

        public void InsertCommand(string cmd)
        {
            
            bool r = ps.Parse(cmd);
            if (r)
            {
                if (ps.LastResult is SiteDefinition)
                    siteDefList.Add(ps.LastResult as SiteDefinition);
                else if (ps.LastResult is TableCreation)
                    tableCreationList.Add(ps.LastResult as TableCreation);
                else if (ps.LastResult is HFragmentation)
                {
                    hFragList.Add(ps.LastResult as HFragmentation);
                    fragmentQueue.Enqueue(ps.LastResult);
                }
                else if (ps.LastResult is VFragmentation)
                {
                    vFragList.Add(ps.LastResult as VFragmentation);
                    fragmentQueue.Enqueue(ps.LastResult);
                }
                else if (ps.LastResult is Allocation)
                    allocationList.Add(ps.LastResult as Allocation);
            }
        }

        /// <summary>
        /// 构造GDD
        /// </summary>
        /// <returns></returns>
        public GlobalDirectory CreateGDD()
        {
            gdd = new GlobalDirectory();
            //先对Site处理
            foreach (SiteDefinition sd in siteDefList)
                gdd.Sites.Add(sd.Site);
            
            //然后创建表格
            foreach(TableCreation tc in tableCreationList)
                gdd.Schemas.Add(tc.Target);
            
            //为每个表格创建一个默认的分片，无条件
            foreach (TableSchema ts in gdd.Schemas)
            {
                Fragment fragment = new Fragment();
                fragment.LogicTable = ts;
                fragment.Condition = null;
                //分片名为表名
                fragment.Name = ts.TableName;
                fragment.Type = FragmentType.None;
                fragment.Schema = ts;

                gdd.Fragments.Add(fragment);
            }

            //构造分片
            CreateFragments();
            
            //分配站点
            AllocateSite();

            return gdd;
        }


        private void AllocateSite()
        {
            foreach (Allocation al in allocationList)
            {
                Fragment f = gdd.Fragments.GetFragmentByName(al.Table.TableName);
                Site s = gdd.Sites[al.Site.Name];
                if (s != null && f != null)
                    f.Site = s;
            }

        }

        /// <summary>
        /// 构造分片，并填充一致性信息
        /// </summary>
        private void CreateFragments()
        {
            //TODO：这里有可能出现死循环

            //分片队列
            while (fragmentQueue.Count > 0)
            {
                object frag = fragmentQueue.Dequeue(); //出队
                Fragment f = null;
                int i = 1;

                if (frag is HFragmentation) //如果是水平分片
                {

                    f = FindParentFragment((frag as HFragmentation).Source.TableName);
                    if (f != null) //找到上层分片
                    {
                        foreach (Condition c in (frag as HFragmentation).FragmentCondition)
                        {
                            Fragment newHFragment = new Fragment();
                            newHFragment.Parent = f;
                            newHFragment.Condition = c;
                            //语意信息
                            newHFragment.Name = (frag as HFragmentation).Source.TableName + "." + i.ToString();
                            i++;
                            newHFragment.Type = FragmentType.Horizontal;
                            f.Children.Add(newHFragment);
                        }
                    }
                    else
                        fragmentQueue.Enqueue(frag); //入队，等待下次
                }
                else if (frag is VFragmentation) //垂直分片
                {
                    f = FindParentFragment((frag as VFragmentation).Source.TableName);
                    if (f != null) //找到上层分片
                    {
                        foreach (TableSchema ts in (frag as VFragmentation).Schemas)
                        {
                            Fragment newVFragment = new Fragment();
                            newVFragment.Parent = f;
                            newVFragment.Name = (frag as VFragmentation).Source.TableName + "." + i.ToString();
                            i++;
                            newVFragment.Type = FragmentType.Vertical;
                            newVFragment.Schema = ts;
                            f.Children.Add(newVFragment);
                        }
                    }
                    else
                        fragmentQueue.Enqueue(frag);//入队，等待下次
                }
            }

            //填写Fragment的一致性信息
            foreach (Fragment topFragment in gdd.Fragments)
                FillFragments(topFragment.LogicTable, topFragment);
        }

        /// <summary>
        /// 用逻辑表填充分片
        /// </summary>
        /// <param name="logicTable"></param>
        /// <param name="f"></param>
        private void FillFragments(TableSchema logicTable,Fragment f)
        {
            if (f.Type != FragmentType.None)
            {
                f.LogicTable = logicTable;
                if (f.Condition != null)
                {
                    ConditionConsistencyFiller filler = new ConditionConsistencyFiller();
                    filler.FillCondition(logicTable, f.Condition);
                }

                //如果是垂直分片，用逻辑表填充分片中的样式
                if (f.Type == FragmentType.Vertical)
                {
                    TableSchema ts = f.Schema;
                    ts.TableName = logicTable.TableName;
                    for (int i = 0; i < ts.Fields.Count;i++ )
                    {
                        //从逻辑表中找到那个字段，然后用其覆盖，因为其信息是最全的
                        Field logicTableField = logicTable[ts.Fields[i].AttributeName];
                        if (logicTableField != null)
                            ts.Fields[i] = logicTableField;
                    }

                    if (ts.Fields.Count < logicTable.Fields.Count)
                        ts.IsAllFields = false;
                }

            }


            //填充孩子
            foreach (Fragment childFrag in f.Children)
                FillFragments(logicTable, childFrag);
        }

        private Fragment FindParentFragmentInFragment(Fragment f,string tableName)
        {
            if (f.Name == tableName)
                return f;

            foreach (Fragment childF in f.Children)
            {
                Fragment f1 = FindParentFragmentInFragment(childF, tableName);
                if (f1 != null)
                    return f1;
            }

            return null;
        }

        private Fragment FindParentFragment(string tableName)
        {
            foreach (Fragment f in gdd.Fragments)
            {
                Fragment resultF = FindParentFragmentInFragment(f, tableName);
                if (resultF != null)
                    return resultF;
            }
            return null;
        }

        
    }
}
