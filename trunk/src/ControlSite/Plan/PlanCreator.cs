using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.RelationalAlgebra.Entity;
using System.Collections;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite.Plan
{
    class PlanCreator
    {
        public ExecutionPlan CreateGlobalPlan(Relation root,string id)
        {
            ExecutionRelation exR = new ExecutionRelation(root, id, -1);
            
            ////////////////生成执行计划//////////////////////////
            ExecutionPlan plan = new ExecutionPlan();
            ExecutionStep step;

            Queue<ExecutionRelation> queue = new Queue<ExecutionRelation>();
            int index = 0;
            queue.Enqueue(exR);

            while (queue.Count > 0)
            {
                Relation re = queue.Dequeue();
                step = new ExecutionStep();
                step.Index = plan.Steps.Count;
                ExecutionRelation joinOrUnion = (re as ExecutionRelation).FindFirstJoinOrUnion(out index);
                if (joinOrUnion != null)
                {
                    step.Operation = new ExecutionRelation(re as ExecutionRelation, index + 1);

                    if (joinOrUnion.Children[0].Children.Count > 0)
                        queue.Enqueue(joinOrUnion.Children[0] as ExecutionRelation);//选择最左边的

                    for (int i = 1; i < joinOrUnion.Children.Count; i++)
                    {
                        queue.Enqueue(joinOrUnion.Children[i] as ExecutionRelation);
                    }
                }
                else
                {
                    step.Operation = new ExecutionRelation(re as ExecutionRelation, -1);
                }
                plan.Steps.Add(step);
            }
            return plan;
        }

        /// <summary>
        /// 将总表分成站点的表
        /// </summary>
        /// <param name="gPlan"></param>
        /// <param name="gdd"></param>
        /// <returns></returns>
        public List<ExecutionPlan> SplitPlan(ExecutionPlan gPlan,GlobalDirectory gdd)
        {
            Hashtable site2PlanTable = new Hashtable();
            Hashtable id2PlanTable = new Hashtable();

            //将一个总计划分成多个站点计划
            for (int i = gPlan.Steps.Count - 1; i >= 0; i--)
            {
                ExecutionRelation leftBottom = FindLeftBottomNode(gPlan.Steps[i].Operation);
                ExecutionPlan currentPlan;

                //如果关系本地数据库表
                if (leftBottom.IsDirectTableSchema) 
                {
                    leftBottom.InLocalSite = true;//设置关系本地站点

                    //找到分片，默认的执行站点
                    Fragment fragment = gdd.Fragments.GetFragmentByName(leftBottom.DirectTableSchema.TableName);
                    if (fragment != null)
                    {
                        leftBottom.DirectTableSchema = fragment.Schema.Clone() as TableSchema;
                        leftBottom.DirectTableSchema.TableName = fragment.LogicTable.TableName;
                        leftBottom.DirectTableSchema.NickName = fragment.Name;

                        currentPlan = (ExecutionPlan)site2PlanTable[fragment.Site.Name];

                        if (currentPlan != null)
                        {
                            currentPlan.Steps.Add(gPlan.Steps[i]);
                            //记录Id与Plan的对应关系
                            id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                        }
                        else
                        {
                            currentPlan = new ExecutionPlan();
                            currentPlan.ExecutionSite = fragment.Site;
                            currentPlan.Steps.Add(gPlan.Steps[i]);
                            site2PlanTable[fragment.Site.Name] = currentPlan;
                            //记录Id与Plan的对应关系
                            id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                        }
                    }
                }
                else 
                {
                    //如果不是直接表，则找到执行这个操作的Plan，然后在这个Plan后面添加步骤
                    currentPlan = (ExecutionPlan)id2PlanTable[leftBottom.ResultID];
                    if (currentPlan != null)
                    {
                        currentPlan.Steps.Add(gPlan.Steps[i]);
                        leftBottom.InLocalSite = true;              //关系本地站点

                        //记录Id与Plan的对应关系
                        id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                    }
                    else //理论上不应该执行到这里，为了保险，先保留
                    {
                        //否则在站点1中做吧，测试
                        currentPlan = (ExecutionPlan)site2PlanTable[gdd.Sites[0].Name];
                        if (currentPlan != null)
                        {
                            currentPlan.Steps.Add(gPlan.Steps[i]);
                            //记录Id与Plan的对应关系
                            id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                        }
                        else
                        {
                            currentPlan = new ExecutionPlan();
                            currentPlan.ExecutionSite = gdd.Sites[0];
                            currentPlan.Steps.Add(gPlan.Steps[i]);
                            site2PlanTable[gdd.Sites[0].Name] = currentPlan;
                            //记录Id与Plan的对应关系
                            id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                        }
                    }
                }

                SetWaitingId(gPlan.Steps[i], leftBottom);


            }

            List<ExecutionPlan> result = new List<ExecutionPlan>();
            foreach (DictionaryEntry entry in site2PlanTable)
                result.Add(entry.Value as ExecutionPlan);

            foreach (ExecutionPlan plan in result)
            {
                foreach (ExecutionStep step in plan.Steps)
                {
                    SetTransferSite(step, result);
                }
            }

            return result;
        }

        private ExecutionRelation FindLeftBottomNode(ExecutionRelation r)
        {
            if (r.Children.Count == 0)
                return r;
            else
                return FindLeftBottomNode(r.Children[0] as ExecutionRelation);
        }

        private void SetWaitingId(ExecutionStep step, ExecutionRelation leftBottom)
        {
            if (leftBottom.Parent != null)
            {
                ExecutionRelation parent = leftBottom.Parent;
                for (int i = 1; i < parent.Children.Count; i++)
                    step.WaitingId.Add((parent.Children[i] as ExecutionRelation).ResultID);
            }
        }

        private void SetTransferSite(ExecutionStep step,List<ExecutionPlan> plans)
        {
            foreach (ExecutionPlan plan in plans)
            {
                foreach (ExecutionStep aStep in plan.Steps)
                {
                    //id2PlanTable[step.Operation.ResultID];
                    if (aStep.IsWaiting(step.Operation.ResultID))
                    {
                        step.TransferSite = plan.ExecutionSite;
                        return;
                    }

                }
            }
        }

    }
}
