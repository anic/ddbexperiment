using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Execution;

namespace DistDBMS.ControlSite
{
    /// <summary>
    /// 这个类是用来临时修改Relation的填写方式，以后不需要用到
    /// </summary>
    class TempModifier
    {
        GlobalDirectory gdd;
        public TempModifier(GlobalDirectory gdd)
        {
            this.gdd = gdd;
        }

        public void Modify(Relation r)
        {
            ModifyProjection(r);
            
        }

        public void CheckLastSchema(ExecutionRelation exR,TableSchema schema)
        {
            if (exR.ResultSchema.Fields.Count > schema.Fields.Count)
            {
                ExecutionRelation project = new ExecutionRelation(exR);
                project.Type = RelationalType.Projection;
                project.RelativeAttributes = schema.Clone() as TableSchema;
                //先定义一个很大的数
                project.ResultID = 100;
                
                
            }
        }

        private void ModifyProjection(Relation r)
        {
            if (r.Type == RelationalType.Projection)
            {
                if (r.Children.Count == 0)
                {
                    if (r.DirectTableSchema.Fields.Count == r.RelativeAttributes.Fields.Count)
                    {
                        r.Type = RelationalType.Selection;
                    }
                    else
                    {
                        Relation selection = new Relation();
                        selection.Type = RelationalType.Selection;
                        selection.DirectTableSchema = r.DirectTableSchema.Clone() as TableSchema;
                        r.DirectTableSchema = null;
                        r.Children.Add(selection);
                    }
                }
            }

            foreach (Relation child in r.Children)
                ModifyProjection(child);
        }

        
        
    }
}
