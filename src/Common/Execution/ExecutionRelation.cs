using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.Common.Execution
{
    public class ExecutionRelation:Relation
    {
        public string ResultID { get; set; }

        public bool InLocalSite { get; set; }

        public ExecutionRelation Parent { get { return parent; } }
        ExecutionRelation parent = null;

        /// <summary>
        /// 先留着，待扩展
        /// </summary>
        public Site ExecutionSite { get; set; }

        /// <summary>
        /// 结果样式
        /// </summary>
        public TableSchema ResultSchema {
            get
            {
                if (IsDirectTableSchema && DirectTableSchema != null
                    && DirectTableSchema.Fields.Count > 0)
                    return DirectTableSchema;

                switch(Type)
                {
                    case RelationalType.Projection:
                        {
                            return RelativeAttributes.Clone() as TableSchema;
                        }
                    case RelationalType.Join:
                        {
                            TableSchema result = new TableSchema();
                            //TODO:这里要考虑是否是同一个表，如果是同一个逻辑表，则join属性合成一个，否则不合成一个
                            for (int i = 0; i < RelativeAttributes.Fields.Count; i++)
                            {
                                if (i % 2 == 0)
                                    result.Fields.Add(RelativeAttributes.Fields[i]);
                            }
                            
                            foreach (ExecutionRelation r in Children)
                            {
                                
                                TableSchema childResult = r.ResultSchema;
                                if (childResult != null)
                                {
                                    if (result.TableName == "")
                                        result.TableName = childResult.TableName;
                                    else
                                    {
                                        //两个表有同样的表名字,表名是同一个表

                                        if (childResult.TableName != result.TableName) //否则A_B_C
                                            result.TableName += "_" + childResult.TableName;
                                    }

                                    foreach(Field f in childResult.Fields)
                                    {
                                        Field searchF = RelativeAttributes[f.AttributeName];
                                        if (searchF == null) //不在相关属性之中
                                            result.Fields.Add(f);
                                    }
                                }
                            }
                            
                            return result;
                        }
                    case RelationalType.Union:
                        {
                            if (Children.Count > 0)
                            {
                                TableSchema result = (Children[0] as ExecutionRelation).ResultSchema.Clone() as TableSchema;
                                if (result!=null)
                                {
                                    int index  = result.TableName.LastIndexOf(".");
                                    if (index!=-1)
                                        result.TableName = result.TableName.Substring(0,index);
                                    return result;
                                }
                            }
                            break;
                        }
                    case RelationalType.Selection:
                        {
                            if (IsDirectTableSchema)
                                return DirectTableSchema;
                            else if (Children.Count > 0)
                                return (Children[0] as ExecutionRelation).ResultSchema;
                               
                            break;
                        }
                    case RelationalType.CartesianProduct:
                        {
                            TableSchema result = null;
                            foreach (ExecutionRelation r in Children)
                            {
                                if (result == null)
                                {
                                    result = r.ResultSchema;
                                    if (result!=null)
                                        result = result.Clone() as TableSchema;
                                }
                                else
                                {
                                    TableSchema tmp = r.ResultSchema;
                                    if (tmp != null)
                                    {
                                        result.Fields.AddRange(tmp.Fields);
                                        result.TableName += "_" + tmp.TableName;
                                    }
                                }
                            }
                            return result;
                        }
                        
                }
                return null;
            }
        }

        public ExecutionRelation(Relation r,string initID,int createLevel)
        {
            ResultID = initID;

            this.Content = r.Content;
            this.DirectTableSchema = r.DirectTableSchema;
            this.IsDirectTableSchema = r.IsDirectTableSchema;
            this.Predication = r.Predication;
            this.RelativeAttributes = r.RelativeAttributes;
            this.Type = r.Type;

            int index = 0;
            if (createLevel -1 >0|| createLevel == -1)
            {
                foreach (Relation child in r.Children)
                {
                    ExecutionRelation exChild = new ExecutionRelation(child, GenerateChildId(index), (createLevel == -1) ? -1 : createLevel - 1);
                    exChild.parent = this;
                    this.Children.Add(exChild);
                    index++;
                }
            }

            this.InLocalSite = false;
            this.ExecutionSite = null;
        }

        public ExecutionRelation(ExecutionRelation r, int createLevel):
            this(r,r.ResultID,createLevel)
        { 

        }

        /// <summary>
        /// 生成子节点ID
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GenerateChildId(int index)
        {
            return ResultID + "." + index.ToString();
        }

        public override string ToString()
        {
            if (!InLocalSite)
                return ResultID + "\t" + base.ToString();
            else
                return ResultID + "*\t" + base.ToString();
        }


        internal class RelationPair
        {
            public Relation relation;
            public int level;
            public RelationPair(Relation r, int level)
            {
                this.relation = r;
                this.level = level;
            }
        }

        public ExecutionRelation FindFirstJoinOrUnion(out int level)
        {
            level = -1;
            Queue<RelationPair> queue = new Queue<RelationPair>();
            queue.Enqueue(new RelationPair(this, 0));
            while (queue.Count > 0)
            {
                RelationPair pair = queue.Dequeue();
                if (pair.relation.Type == RelationalType.Join || pair.relation.Type == RelationalType.Union)
                {
                    level = pair.level;
                    return pair.relation as ExecutionRelation;
                }
                else
                {
                    foreach (Relation child in pair.relation.Children)
                        queue.Enqueue(new RelationPair(child, pair.level + 1));
                }
            }
            return null;
        }

        

    }
}
