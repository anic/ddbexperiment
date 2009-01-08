using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.ControlSite.RelationalAlgebraUtility;

namespace DistDBMS.ControlSite.Plan
{
    class DataDeletor
    {
        GlobalDirectory gdd;

        List<ExecutionPlan> resultPlans;

        public DataDeletor(GlobalDirectory gdd)
        {
            this.gdd = gdd;
            resultPlans = new List<ExecutionPlan>();
        }

        public List<ExecutionPlan> DeleteFromSql(Deletion deletion)
        {

            resultPlans.Clear();

            List<Fragment> fragments = GetFragments(deletion.Source, deletion.Condition);
            
            foreach (Fragment f in fragments)
            {
                ExecutionPlan plan = GetPlanBySite(f.Site);
                ExecutionStep step = new ExecutionStep();

                step.Type = ExecutionStep.ExecuteType.Delete;
                step.Index = plan.Steps.Count;
                step.Operation = new ExecutionRelation();
                step.Operation.DirectTableSchema = deletion.Source.Clone() as TableSchema;
                step.Operation.DirectTableSchema.ReplaceTableName(f.Name);
                step.Operation.Predication = deletion.Condition.Clone() as Condition;
                plan.Steps.Add(step);
            }
            return resultPlans;

        }

        /// <summary>
        /// 获取分片的所有约束条件谓词
        /// </summary>
        /// <param name="fragment"></param>
        /// <returns></returns>
        private List<AtomCondition> GetFragmentConditions(Fragment fragment)
        {
            List<AtomCondition> results = new List<AtomCondition>();

            if (fragment == null || fragment.Condition == null || fragment.Condition.IsEmpty)
                return results;

            ConditionConverter conditionConverter = new ConditionConverter();
            conditionConverter.Convert(fragment.Condition, NormalFormType.Conjunction);
            ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;
            results.AddRange(normalForm.PredicationItems);

            results.AddRange(GetFragmentConditions(fragment.Parent));

            return results;
        }

        /// <summary>
        /// 在GDD中查找所有与tuple不冲突的Fragment
        /// </summary>
        /// <param name="fragment"></param>
        /// <param name="tupleAttributeConditions"></param>
        /// <param name="result"></param>
        private void GetUnconflictFragments(Fragment fragment, List<AtomCondition> tupleAttributeConditions, ref List<Fragment> result)
        {
            // 叶子结点
            if (fragment.Children.Count == 0)
            {
                List<AtomCondition> fragmentConditions = GetFragmentConditions(fragment);
                foreach (AtomCondition fragmentAtom in fragmentConditions)
                {
                    foreach (AtomCondition tupleAtom in tupleAttributeConditions)
                    {
                        if (fragmentAtom.ConflictWith(tupleAtom))
                            return;
                    }
                }

                result.Add(fragment);
            }
            else // 中间分片结点
            {
                foreach (Fragment f in fragment.Children)
                    GetUnconflictFragments(f, tupleAttributeConditions, ref result);
            }
            
        }

        public List<Fragment> GetFragments(TableSchema schema, Condition condition)
        {
            List<Fragment> result = new List<Fragment>();

            List<AtomCondition> predications = new List<AtomCondition>();

            ConditionConverter conditionConverter = new ConditionConverter();
            conditionConverter.Convert(condition, NormalFormType.Conjunction);
            ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;

            if (normalForm != null)
                predications.AddRange(normalForm.PredicationItems);

            foreach (AtomCondition atom in predications)
            {
                atom.Normalize();
                atom.LeftOperand.Field.TableName = schema.TableName;
            }

            foreach (Fragment fragment in gdd.Fragments)
            {
                if (fragment.Name.Equals(schema.TableName))
                {
                    GetUnconflictFragments(fragment, predications, ref result);
                    break;
                }
            }

            return result;

            /*

            if (schema.TableName == "Producer")
            {
                result.Add(gdd.Fragments.GetFragmentByName("Producer.2"));
                result.Add(gdd.Fragments.GetFragmentByName("Producer.4"));
            }
            else if (schema.TableName == "Customer")
            {
                result.Add(gdd.Fragments.GetFragmentByName("Customer.1"));
                result.Add(gdd.Fragments.GetFragmentByName("Customer.2"));
                result.Add(gdd.Fragments.GetFragmentByName("Customer.3"));
            }

            return result;
             * */
        }

        private ExecutionPlan GetPlanBySite(Site site)
        {
            foreach (ExecutionPlan plan in resultPlans)
                if (plan.ExecutionSite.Name == site.Name)
                    return plan;

            ExecutionPlan newPlan = new ExecutionPlan();
            newPlan.ExecutionSite = site;
            resultPlans.Add(newPlan);
            return newPlan;
        }
    }
}
