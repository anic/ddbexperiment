using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;

using DistDBMS.Common.Table;


namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class Relation:ICloneable
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

        /// <summary>
        /// 有效属性
        /// 此节点的父节点至根结点所涉及的所有属性
        /// </summary>
        public List<Field> EffectiveAttributes;

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

        public object Clone()
        {
            Relation r = new Relation();
            r.Type = this.Type;
            
            if (this.DirectTableSchema != null)
                r.DirectTableSchema = this.DirectTableSchema.Clone() as TableSchema;
            
            /*
            if (this.LeftRelation != null)
                r.LeftRelation = this.LeftRelation.Clone() as Relation;
            
            if (this.RightRelation != null)
                r.RightRelation = this.RightRelation.Clone() as Relation;
            */

            if (this.ResultName != null)
                r.ResultName = this.ResultName.Clone() as String;
            
            if (this.RelativeAttributes != null)
                r.RelativeAttributes = this.RelativeAttributes.Clone() as TableSchema;
            
            if (this.Predication != null)
                r.Predication = this.Predication.Clone() as Condition;

            foreach (Relation child in Children)
                r.Children.Add(child.Clone() as Relation);

            return r;            
        }

        public void Copy(Relation r)
        {
            this.Type = r.Type;
            this.DirectTableSchema = r.DirectTableSchema;
            this.LeftRelation = r.LeftRelation;
            this.RightRelation = r.RightRelation;
            this.ResultName = r.ResultName;
            this.RelativeAttributes = r.RelativeAttributes;
            this.Predication = r.Predication;

            this.children = new List<Relation>();
            foreach (Relation child in r.Children)
                this.Children.Add(child);            
        }

        public string TypeName
        {
            get
            {
                string type = "";
                switch (Type)
                {
                    case RelationalType.Selection:
                        type = "Select";
                        break;
                    case RelationalType.Projection:
                        type = "Project";
                        break;
                    case RelationalType.Join:
                        type = "Join";
                        break;
                    case RelationalType.CartesianProduct:
                        type = "Cart";
                        break;
                    case RelationalType.Union:
                        type = "Union";
                        break;
                    case RelationalType.Semijoin:
                        type = "SemiJoin";
                        break;
                }
                return type;
            }
        }

        private string toString(int indent)
        {
            string str = "";

            for (int i = 0; i < indent; i++)
                str += " ";


            switch (Type)
            {
                case RelationalType.Projection:
                    str = str + Type + "[";
                    foreach (Field field in RelativeAttributes.Fields)
                        str = str + " " + field.TableName + "." + field.AttributeName;
                    str += "]\n";
                    break;
                case RelationalType.Selection:
                    str = str + Type + "[" + this.Predication.ToString() + "]" + "\n";
                    break;
                case RelationalType.Union:
                    str = str + Type + "\n";
                    break;
                case RelationalType.Join:
                    str = str + Type + "\n";
                    break;
            }
            

            //if (IsDirectTableSchema)
            //    return str;

            foreach (Relation child in Children)
            {
                str += child.toString(indent + 4);
            }
            return str;
        }

        public string toString()
        {
            return toString(0);
        }
    }
}
