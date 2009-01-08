using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.RelationalAlgebra.Entity;
using System.Collections;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra;

namespace DistDBMS.ControlSite.Plan
{
    class QueryPlanCreator
    {
        GlobalDirectory gdd;
        ExecutionRelation exR;

        public QueryPlanCreator(GlobalDirectory gdd)
        {
            this.gdd = gdd;
        }

        public ExecutionRelation LastResult {
            get { return exR; }
        }

        /// <summary>
        /// 生成全局的Plan
        /// </summary>
        /// <param name="root"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExecutionPlan CreateGlobalPlan(Relation root,int id)
        {
            exR = new ExecutionRelation(root, ref id, -1);


            
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
                
                //TODO:这里应该修改为找到分枝节点，且其孩子节点包含在两个站点中
                //其实就只有Join和Union可以有子孩子
                //ExecutionRelation splitRelation = (re as ExecutionRelation).FindFirstJoinOrUnion(out index);
                ExecutionRelation splitRelation = FindSplitRelation(re as ExecutionRelation, out index);
                
                if (splitRelation != null)
                {
                    step.Operation = new ExecutionRelation(re as ExecutionRelation, index + 1);

                    for (int i = 0; i < splitRelation.Children.Count; i++)
                    {
                        if (
                            (splitRelation.Children.Count == 1)                 //只有一个分枝
                            || (i > 0)      //非第一个分枝
                            || (i == 0 && !splitRelation.Children[i].IsDirectTableSchema) //第一个分枝且不是直接关系数据库表
                            )
                            queue.Enqueue(splitRelation.Children[i] as ExecutionRelation);
                        //bool result = !IsSingleTableSelectionAndProjection(splitRelation.Children[i] as ExecutionRelation);
                        //bool result2 = !splitRelation.Children[i].IsDirectTableSchema;
                        //if (result != result2 && i == 0)
                        //{
                        //    int a = 0;
                        //}

                        //if (
                        //    (splitRelation.Children.Count == 1)                 //只有一个分枝
                        //    || (i > 0)      //非第一个分枝
                        //    || (i == 0 && !IsSingleTableSelectionAndProjection(splitRelation.Children[i] as ExecutionRelation)) //第一个分枝且不是直接关系数据库表
                        //    )
                        //    queue.Enqueue(splitRelation.Children[i] as ExecutionRelation);
                    }
                }
                else //如果没有分裂点，表明从这个开始到底层都是一个step
                {
                    step.Operation = new ExecutionRelation(re as ExecutionRelation, -1);    
                }
                plan.Steps.Add(step);
            }
            return plan;
        }

        /// <summary>
        /// 是否是单个表的投影和选择
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool IsSingleTableSelectionAndProjection(ExecutionRelation r)
        {
            if (r.Children.Count == 0 && r.IsDirectTableSchema)
                return true;

            if (r.Children.Count >= 2)
                return false;

            //有一个孩子
            if (r.Type == RelationalType.Selection || r.Type == RelationalType.Projection)
            {
                return IsSingleTableSelectionAndProjection(r.Children[0] as ExecutionRelation);
            }
            else
                return false;
        }


        Hashtable site2PlanTable = new Hashtable();
        Hashtable id2PlanTable = new Hashtable();
        List<ExecutionPlan> resultPlans = new List<ExecutionPlan>();

        /// <summary>
        /// 将总表分成站点的表
        /// </summary>
        /// <param name="gPlan"></param>
        /// <param name="gdd"></param>
        /// <returns></returns>
        public List<ExecutionPlan> SplitPlan(ExecutionPlan gPlan)
        {
            resultPlans.Clear();
            site2PlanTable.Clear();
            id2PlanTable.Clear();

            //将一个总计划分成多个站点计划
            for (int i = gPlan.Steps.Count - 1; i >= 0; i--)
            {
                ExecutionRelation leftBottom = FindLeftBottomNode(gPlan.Steps[i].Operation);
                ExecutionPlan currentPlan;

                //如果关系本地数据库表
                if (leftBottom.IsDirectTableSchema) 
                {
                    

                    //找到分片，默认的执行站点
                    Fragment fragment = gdd.Fragments.GetFragmentByName(leftBottom.DirectTableSchema.TableName);
                    if (fragment != null && fragment.Site!=null)
                    {
                  
                        leftBottom.DirectTableSchema = fragment.Schema.Clone() as TableSchema;
                        //leftBottom.DirectTableSchema.TableName = fragment.LogicSchema.TableName;
                        //leftBottom.DirectTableSchema.NickName = fragment.Name;

                        currentPlan = GetPlanBySite(fragment.Site);
                        currentPlan.Steps.Add(gPlan.Steps[i]);
                        //记录Id与Plan的对应关系
                        id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;

                        
                        leftBottom.InLocalSite = true;
                        //将所有的父亲标记为InLocalSite
                        ExecutionRelation parent = leftBottom.Parent;
                        while (parent != null)
                        {
                            parent.InLocalSite = true;
                            parent = parent.Parent;
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
                        gPlan.Steps[i].WaitingId.Add(leftBottom.ResultID);

                        //记录Id与Plan的对应关系
                        id2PlanTable[gPlan.Steps[i].Operation.ResultID] = currentPlan;
                    }
                }

                SetBottomInfo(gPlan.Steps[i], leftBottom);
            }

            foreach (ExecutionPlan plan in resultPlans)
                foreach (ExecutionStep step in plan.Steps)
                    SetTransferSite(step, resultPlans);
            

            return resultPlans;
        }


        private ExecutionStep GetStepById(int id)
        {
            ExecutionPlan plan = (ExecutionPlan)id2PlanTable[id];
            if (plan != null)
            {
                foreach (ExecutionStep step in plan.Steps)
                    if (step.Operation != null && step.Operation.ResultID == id)
                        return step;
            }
            return null;
        }
        
        /// <summary>
        /// 通过站点获得Plan
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        private ExecutionPlan GetPlanBySite(Site site)
        {
            ExecutionPlan result = (ExecutionPlan)site2PlanTable[site.Name];
            if (result == null)
            {
                result = new ExecutionPlan();
                result.ExecutionSite = site;
                site2PlanTable[site.Name] = result;
                resultPlans.Add(result);
            }
            return result;
        }

        private ExecutionPlan GetPlanByStep(ExecutionStep target)
        {
            foreach (ExecutionPlan plan in resultPlans)
            {
                foreach (ExecutionStep step in plan.Steps)
                    if (step == target)
                        return plan;
            }
            return null;
        }

        /// <summary>
        /// 找到最左的底层节点
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private ExecutionRelation FindLeftBottomNode(ExecutionRelation r)
        {
            if (r.Children.Count == 0)
                return r;
            else
                return FindLeftBottomNode(r.Children[0] as ExecutionRelation);
        }

        /// <summary>
        /// 设置底层节点的信息，包括等待ID
        /// </summary>
        /// <param name="step"></param>
        /// <param name="leftBottom"></param>
        private void SetBottomInfo(ExecutionStep step, ExecutionRelation leftBottom)
        {
            if (leftBottom.Parent != null)
            {
                ExecutionRelation parent = leftBottom.Parent ;
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    ExecutionRelation currentRelation = (parent.Children[i] as ExecutionRelation);
                    //应该在这里判断是否本地操作
                    if (!currentRelation.InLocalSite)                    //i==0 是默认本地操作
                        step.WaitingId.Add(currentRelation.ResultID);

                    //在这里把非本地的数据的结构记录
                    //需要填写fragment中的信息
                    string nickName = "";
                    
                    if (parent.Children[i].IsDirectTableSchema)
                        nickName = parent.Children[i].DirectTableSchema.TableName;

                    ExecutionStep tmpStep = GetStepById(currentRelation.ResultID);
                    if (tmpStep != null)
                    {
                        //修改那些非直接表格
                        
                        currentRelation.DirectTableSchema = tmpStep.Operation.ResultSchema;
                        if (nickName != "")
                            currentRelation.DirectTableSchema.NickName = nickName;

                        if (currentRelation.DirectTableSchema.TableName == "") //如果表名没有，则设置为nickname
                            currentRelation.DirectTableSchema.TableName = nickName;
                        
                    }
                    
                }
            }
        }


        /// <summary>
        /// 设置传送站点
        /// </summary>
        /// <param name="step"></param>
        /// <param name="plans"></param>
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

        /// <summary>
        /// 查找分裂的节点
        /// </summary>
        /// <param name="r"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected ExecutionRelation FindSplitRelation(ExecutionRelation r,out int level)
        {
            //这里用到的策略是，碰到分成2枝的就作为分裂的点
            level = 1;
            if (r.Children.Count > 1 )
                return r;
            else if (r.Children.Count == 1)
            {
                if (r.Children[0].Type == RelationalType.Union)
                    return r;
                else
                {
                    int tempLevel;
                    ExecutionRelation result = FindSplitRelation(r.Children[0] as ExecutionRelation, out tempLevel);
                    level = tempLevel + 1;
                    return result;
                }
            }
            else
                return null;
        }

        public void FillSite(ExecutionRelation r, List<ExecutionPlan> plans)
        {
            foreach (ExecutionPlan plan in plans)
            {
                foreach (ExecutionStep step in plan.Steps)
                {
                    if (step.Operation is ExecutionRelation)
                        VisitRelation(step.Operation as ExecutionRelation, r, plan.ExecutionSite);
                }
            }
        }

        private void VisitRelation(ExecutionRelation r,ExecutionRelation target,Site site)
        {
            ExecutionRelation result = target.FindRelationById(r.ResultID);
            if (result != null)
            {
                if (r.InLocalSite)
                    result.ExecutionSite = site;
                else if (result.ExecutionSite == null)
                    result.ExecutionSite = site; //先填写一个上去
            }

            foreach (ExecutionRelation child in r.Children)
                VisitRelation(child, target,site);

        }

    }
}
