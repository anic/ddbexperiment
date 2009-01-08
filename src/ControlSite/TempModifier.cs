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
            //ModifyRelativeAttribute(r);
            //ModifySelection(r);
            //ModifyUnion(r);
            //ModifyJoin(r);
            ModifyProjection(r);
            //ModifyForTest(r);
        }

        private void ModifyForTest(Relation r)
        {
            if (r.Type == RelationalType.Projection
                && r.RelativeAttributes.Fields.Count == 1)
                //&& r.RelativeAttributes.Fields[0].TableName == "Product"
                //&& r.RelativeAttributes.Fields[1].TableName == "Product.2")
            {
//                r.RelativeAttributes.Fields[0].TableName = "Product.1";
                Field newField = new Field();
                newField.TableName = "Product.1";
                newField.AttributeName = "id";
                TableSchema old = r.RelativeAttributes;
                r.RelativeAttributes = new TableSchema();
                r.RelativeAttributes.Fields.Add(newField);
                r.RelativeAttributes.Fields.Add(old.Fields[0]);

                int a = 0;
            }

            if (r.Children.Count == 2 && r.Children[1].Type == RelationalType.Selection
                && r.Children[1].IsDirectTableSchema && r.Children[1].DirectTableSchema.TableName == "Product.2")
            {
                Relation old = r.Children[1];
                r.Children[1] = old.Children[0];
                old.Children[0].ResultName = "Product.2";
            }

            foreach (Relation child in r.Children)
                ModifyForTest(child);
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

        private void ModifyRelativeAttribute(Relation r)
        {
            if (r.RelativeAttributes != null && r.RelativeAttributes.Fields.Count > 0)
            {
                for (int i = 0; i < r.RelativeAttributes.Fields.Count; i++)
                {
                    Field f = r.RelativeAttributes.Fields[i];
                    TableSchema logicSchema = gdd.Schemas[f.TableName];
                    if (logicSchema != null) //如果有对应的，则修改，没有对应的，如Course.1.id，则不修改
                    {
                        r.RelativeAttributes.Fields[i] = logicSchema[f.AttributeName].Clone() as Field;
                    }
                }
            }
            foreach (Relation child in r.Children)
                ModifySelection(child);
        }

        private void ModifySelection(Relation r)
        {
            if (r.Type == RelationalType.Selection)
            {
                if (r.Children.Count == 0 && r.IsDirectTableSchema && r.RelativeAttributes.Fields.Count > 0)
                    r.DirectTableSchema = r.RelativeAttributes.Clone() as TableSchema;
            }

            foreach (Relation child in r.Children)
                ModifySelection(child);
        }

        private void ModifyUnion(Relation r)
        {

            if (r.Type==RelationalType.Union)
            {
                string name = FindChildSchemaName(r.Children[0]);
                int index = name.LastIndexOf('.');
                if (index != -1)
                    r.ResultName = name.Substring(0, index);
                else   //不会到这里
                    r.ResultName = name;
            }

            foreach (Relation child in r.Children)
                ModifyUnion(child);
                
        }

        private void ModifyJoin(Relation r)
        {
            if (r.Type == RelationalType.Join)
            { 
                string name1 = FindChildSchemaName(r.Children[0]);
                string name2 = FindChildSchemaName(r.Children[1]);
                int index1 = name1.LastIndexOf('.');
                int index2 = name2.LastIndexOf('.');
                string logic1,logic2;
                
                if (index1 != -1)
                    logic1 = name1.Substring(0, index1);
                else
                    logic1 = name1;

                if (index2 != -1)
                    logic2 = name2.Substring(0, index1);
                else
                    logic2 = name2;

                if (logic1 == logic2)
                    r.ResultName = logic1;

            }

            foreach (Relation child in r.Children)
                ModifyJoin(child);
        }

        private string FindChildSchemaName(Relation r)
        {
            if (r.IsDirectTableSchema)
                return r.DirectTableSchema.TableName;

            if (r.ResultName != "")
                return r.ResultName;

            return FindChildSchemaName(r.Children[0]);
        }

        
    }
}
