﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;

using DistDBMS.Common.Table;


namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class Relation
    {
        /// <summary>
        /// 关系代数的关系类型，如Selection,Projection
        /// </summary>
        public RelationalType Type { get; set; }

        /// <summary>
        /// 子关系
        /// </summary>
        public List<Relation> Children { get { return children; } } //修改过
        List<Relation> children;

        /// <summary>
        /// 获得左关系
        /// </summary>
        public Relation LeftRelation
        {
            get
            {
                if (children.Count == 0)
                    return null;
                else
                    return children[0];
            }
            set
            {
                if (children.Count == 0)
                    children.Add(value);
                else
                    children[0] = value;
            }
        }


        /// <summary>
        /// 获得右关系
        /// </summary>
        public Relation RightRelation
        {
            get
            {
                if (children.Count == 0 || children.Count == 1)
                    return null;
                else
                    return children[children.Count - 1];
            }
            set
            {
                if (children.Count == 0 || children.Count == 1)
                    children.Add(value);
                else //关系多于2
                    children[children.Count - 1] = value;
            }
        }

        /// <summary>
        /// 是否关系直接的表
        /// </summary>
        public bool IsDirectTableSchema { get { return DirectTableSchema != null; } }

        /// <summary>
        /// 如果关系直接的表，则获得原子的表
        /// </summary>
        public TableSchema DirectTableSchema { get; set; }

        /// <summary>
        /// 谓词，如果有
        /// </summary>
        public Condition Predication { get; set; }

        /// <summary>
        /// 相关的属性集
        /// </summary>
        public TableSchema RelativeAttributes { get; set; }

        /// <summary>
        /// 结果的名称，如果为“”，则不修改表名；如果不为“”，则修改表名。
        /// </summary>
        public string ResultName { get; set; }

        public Relation()
        {

            children = new List<Relation>();
            DirectTableSchema = null;
            RelativeAttributes = new TableSchema();
            Predication = new Condition();
            Type = RelationalType.Selection;

            ResultName = "";
        }

        public static Relation EmptyRelation
        {
            get { return null; }
        }

        public override string ToString()
        {
            string result = Type.ToString() + ":";
            if (IsDirectTableSchema)
                result += " " + DirectTableSchema.ToString();

            if (ResultName != "")
                result += " as " + ResultName;

            if (Predication != null && !Predication.IsEmpty
                && Type == RelationalType.Selection)
                result += " Predication: " + Predication.ToString();

            if ((RelativeAttributes.Fields.Count > 0 || RelativeAttributes.TableName != "")
                && (Type == RelationalType.Join || Type == RelationalType.Projection))
                result += " Attributes: " + RelativeAttributes.ToString();


            return result;
        }

    }
}
