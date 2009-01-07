using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Table;
using System.Collections;
using DistDBMS.ControlSite.SQLSyntax.Operation;

namespace DistDBMS.ControlSite.Plan
{
    class ImportPlanCreator
    {
        GlobalDirectory gdd;
        List<ExecutionPlan> resultPlans;
        
        public ImportPlanCreator(GlobalDirectory gdd)
        {
            this.gdd = gdd;
            resultPlans = new List<ExecutionPlan>();
        
        }

        public List<ExecutionPlan> CreatePlans(DataImporter importer)
        {
            resultPlans.Clear();
        
            foreach (Table table in importer.Tables)
            {
                Fragment fragment = gdd.Fragments.GetFragmentByName(table.Schema.TableName);
                ExecutionPlan plan = GetPlanBySite(fragment.Site);

                ExecutionStep step = new ExecutionStep();
                step.Type = ExecutionStep.ExecuteType.Insert;
                step.Table = table;
                plan.Steps.Add(step);
            }
            return resultPlans;
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
