﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;

namespace DistDBMS.ControlSite
{
    /// <summary>
    /// 这个类是用来临时修改Relation的填写方式，以后不需要用到
    /// </summary>
    class TempModifier
    {
        public void Modify(Relation r)
        {
            ModifySelection(r);
            ModifyUnion(r);
            ModifyJoin(r);
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
