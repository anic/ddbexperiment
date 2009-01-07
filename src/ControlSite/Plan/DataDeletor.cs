using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.Common.Syntax;

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

        public List<Fragment> GetFragments(TableSchema schema, Condition condition)
        {
            List<Fragment> result = new List<Fragment>();
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
