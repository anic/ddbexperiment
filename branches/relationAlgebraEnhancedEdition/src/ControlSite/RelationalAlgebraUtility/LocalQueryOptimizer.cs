using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Dictionary;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;
using DistDBMS.ControlSite.SQLSyntax.Operation;

namespace DistDBMS.ControlSite.RelationalAlgebraUtility
{
    class LocalQueryOptimizer
    {
        private Relation queryTree;
        private GlobalDirectory gdd;
        private List<AtomCondition> unaryPredictions;
        private List<AtomCondition> binaryPredictions;
        private List<Fragment> fragments;
        private TableSchemaList tables;
        private FieldList projectAttributes;

        public LocalQueryOptimizer(Relation tree, Selection select, GlobalDirectory gdd)
        {
            queryTree = tree;
            this.gdd = gdd;

            unaryPredictions = new List<AtomCondition>();
            binaryPredictions = new List<AtomCondition>();

            fragments = new List<Fragment>();
            
            tables = new TableSchemaList();
            tables.AddRange(select.Sources);

            projectAttributes = new FieldList();
            projectAttributes.AddRange(select.Fields.Fields);



            // 分类谓词
            DissectCondition(select.Condition);

            //ClassifyPrediction();

            /*
            System.Console.WriteLine("*********************************");
            System.Console.WriteLine("\nUnaryPrediction:\n");
            foreach (AtomCondition atom in unaryPredictions)
                System.Console.WriteLine(atom.ToString());
            System.Console.WriteLine("\n\nBinaryPrediction:\n");
            foreach (AtomCondition atom in binaryPredictions)
                System.Console.WriteLine(atom.ToString());
            System.Console.WriteLine("*********************************");
            */

            // Join联通分析
            List<TableSchemaList> connectRelations = new List<TableSchemaList>();
            TableSchemaList singleRelations = new TableSchemaList();
            AnalyseRelationJoin(ref connectRelations, ref singleRelations);

            System.Console.WriteLine("");
            System.Console.WriteLine("************************************");
            System.Console.WriteLine("-------Connected Relation----");
            for (int g = 0; g < connectRelations.Count; g++)
            {
                System.Console.WriteLine("Connect gourp[" + g.ToString() + "]:");
                foreach (TableSchema ts in connectRelations[g])
                {
                    System.Console.Write(ts.Tag+"_"+ts.TableName + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine("");
            System.Console.WriteLine("--------Single Relation------");
            foreach (TableSchema ts in singleRelations)
            {
                System.Console.Write(ts.Tag + "_" + ts.TableName + " ");
            }
            
            

            // 对于非联通的Table，有效属性为：投影属性、一元谓词属性
            List<Relation> singleTrees = new List<Relation>();
            if (singleRelations.Count > 0)
            {
                
                TranslateSingleRelationCollection(singleRelations, ref singleTrees);

                foreach (Relation r in singleTrees)
                {
                    System.Console.Write(r.toString());
                }
            }

            // System.Console.WriteLine("***********************************************");

            // 对于联通的Table，有效属性为：join条件、投影属性、一元谓词
            List<Relation> connectedTrees = new List<Relation>();
            if (connectRelations.Count > 0)
            {
                TranslateConnectedRelationCollection(connectRelations, ref connectedTrees);
            }

            // 对每个有用表，在一元谓词中查找所属谓词并挂在Selection上

            // 根据GDD排序，然后Join

            // 最上层Cart

        }

        /// <summary>
        /// 联通Table转换
        /// </summary>
        /// <param name="connectedCollection">联通Table集合</param>
        /// <param name="subTrees">各联通集转化而成的关系代数树</param>
        private bool TranslateConnectedRelationCollection(List<TableSchemaList> connectedCollection, ref List<Relation> subTrees)
        {
            if (connectedCollection == null)
                return false;

            foreach (TableSchemaList list in connectedCollection)
            {
                Relation r = new Relation();
                GenConnectRelationTree(list, ref r);
                subTrees.Add(r);
            }

            return true;
        }

       
        /// <summary>
        /// 将一个联通集内的Table转换为关系代数树形式
        /// 对于联通的Table，有效属性为：join条件、投影属性、一元谓词
        /// </summary>
        /// <param name="connectedTables"></param>
        /// <param name="tree"></param>
        private void GenConnectRelationTree(TableSchemaList connectedTables, ref Relation tree)
        {
            List<Relation> joinElements = new List<Relation>();

            foreach (TableSchema ts in connectedTables)
            {
                Relation r = new Relation();
                GenSingleRelationTree(ts, true, ref r);
                joinElements.Add(r);

                System.Console.Write(r.toString());
            }

            // TODO: 将joinElements中的各个元素join起来
            // 在做完Localization之后再评估各分支，然后join
            //

        }

        /// <summary>
        /// join一个联通集内的每个Relation子树
        /// </summary>
        /// <param name="joinElements">经过Selection和Projection的Relation的集合</param>
        /// <param name="joinResult">join后的根结点</param>
        private void JoinRelations(List<Relation> joinElements, ref Relation joinResult)
        {
 
        }



        /// <summary>
        /// 将Condition转换为合取范式
        /// 并将各谓词分类为一元谓词和二元谓词
        /// </summary>
        /// <param name="predications"></param>
        private void DissectCondition(Condition predications)
        {
            if (predications == null)
                return;

            ConditionConverter conditionConverter = new ConditionConverter();
            conditionConverter.Convert(predications, NormalFormType.Conjunction);
            ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;

            foreach (AtomCondition atomPredication in normalForm.PredicationItems)
            {
                if (atomPredication.LeftOperand.IsField)
                {
                    //左操作数为属性
                    if (atomPredication.RightOperand.IsField)
                    {
                        //二元谓词
                        binaryPredictions.Add(atomPredication);
                    }
                    else
                    {
                        //一元谓词
                        unaryPredictions.Add(atomPredication);
                    }

                }
                else
                {
                    //一元谓词
                    unaryPredictions.Add(atomPredication);
                }
            } 
        }

        /// <summary>
        /// 非联通Table转换
        /// </summary>
        /// <param name="singleRelations"></param>
        /// <param name="trees"></param>
        private void TranslateSingleRelationCollection(TableSchemaList singleRelations, ref List<Relation> subTrees)
        {
            Relation r;
            foreach (TableSchema ts in singleRelations)
            {
                r = new Relation();
                GenSingleRelationTree(ts, false, ref r);
                subTrees.Add(r);
            }
        }

        /// <summary>
        /// 对于非联通的Table，有效属性为：投影属性、相关一元谓词属性
        /// 若投影属性与一元谓词属性均为空，则投影主键
        /// 
        /// 对于联通的Table，有效属性为：join条件、投影属性、一元谓词
        /// 
        /// 
        /// 计算全局有效属性，确定单独Relation是否有用
        /// -selection，Projection，最后和join后的结果Cart
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tree"></param>
        private void GenSingleRelationTree(TableSchema table, bool isConnected, ref Relation tree)
        {
            FieldList projectFields = new FieldList();
            FieldList selectField = new FieldList();
            FieldList joinField = new FieldList();

            // 与table相关的二元谓词
            List<AtomCondition> joinCondition = new List<AtomCondition>();

            // 与table相关的一元谓词集合
            List<AtomCondition> predictions = new List<AtomCondition>();

            FieldList tableFields = new FieldList();
            tableFields.AddRange(table.Fields);

            // 提取table中的投影属性
            foreach (Field f in projectAttributes)
            {
                if (tableFields[f] != null && projectFields[f] == null)
                    projectFields.Add(f.Clone() as Field);
            }

            // 提取相关一元谓词属性
            foreach (AtomCondition atom in unaryPredictions)
            {
                if (atom.LeftOperand.IsField && tableFields[atom.LeftOperand.Field] != null && selectField[atom.LeftOperand.Field] == null)
                {
                    predictions.Add(atom);
                    selectField.Add(atom.LeftOperand.Field.Clone() as Field);
                    continue;
                }

                if (atom.RightOperand.IsField && tableFields[atom.RightOperand.Field] != null && selectField[atom.RightOperand.Field] == null)
                {
                    predictions.Add(atom);
                    selectField.Add(atom.RightOperand.Field.Clone() as Field);
                }
            }


            // 联通集合，提取二元谓词属性
            if (isConnected)
            {
                foreach (AtomCondition atom in binaryPredictions)
                {
                    if (tableFields[atom.LeftOperand.Field] != null && joinField[atom.LeftOperand.Field] == null)
                    {
                        joinCondition.Add(atom);
                        joinField.Add(atom.LeftOperand.Field as Field);
                    }
                    else if (tableFields[atom.RightOperand.Field] != null && joinField[atom.RightOperand.Field] == null)
                    {
                        joinCondition.Add(atom);
                        joinField.Add(atom.RightOperand.Field as Field);
                    }
                }
            }

            // 构建投影节点
            tree.Type = RelationalType.Projection;
            tree.ResultName = table.TableName.Clone() as string;

            if (!isConnected) // 非联通Table，Project主键或投影属性
            {
                if (projectFields.Count == 0)   // 无任何有效属性，投影主键
                    tree.RelativeAttributes.Fields.Add(table.PrimaryKeyField);
                else  // 投影属性
                    tree.RelativeAttributes.Fields.AddRange(projectFields);
            }
            else // 联通Table，Project投影属性和join属性
            {
                tree.RelativeAttributes.Fields.AddRange(joinField);
                tree.RelativeAttributes.Fields.AddRange(projectFields);
            }

            // 如没有Select谓词，直接在Project上挂TableSchema
            if (selectField.Count == 0)
            {
                tree.DirectTableSchema = table.Clone() as TableSchema;
                return;
            }

            /**
             * 存在select谓词，构建Select节点
             */
            Relation active = tree;

            // Selection
            foreach (AtomCondition atom in predictions)
            {
                Relation r = new Relation();
                r.Type = RelationalType.Selection;
                r.Predication = new Condition();
                r.Predication.AtomCondition = atom.Clone() as AtomCondition;
                r.ResultName = table.TableName.Clone() as string;

                active.Children.Add(r);
                active = r;
            }
            active.DirectTableSchema = table.Clone() as TableSchema;
        }

        // private void 



        /// <summary>
        /// 将谓词为一元谓词和二元谓词两类
        /// </summary>
        private void ClassifyPrediction()
        {
            Relation r = queryTree.Children[0];

            while (r.Type == RelationalType.Selection && !r.IsDirectTableSchema)
            {
                if (r.Predication.AtomCondition.LeftOperand.IsField)
                {
                    //左操作数为属性
                    if (r.Predication.AtomCondition.RightOperand.IsField)
                    {
                        //二元谓词
                        binaryPredictions.Add(r.Predication.AtomCondition);
                    }
                    else
                    { 
                        //一元谓词
                        unaryPredictions.Add(r.Predication.AtomCondition);
                    }

                }
                else
                {
                    //一元谓词
                    unaryPredictions.Add(r.Predication.AtomCondition);
                }

                r = r.Children[0];
            }
        }

        /// <summary>
        /// Join图分析
        /// </summary>
        /// <param name="connectRelations">各个联通集的集合</param>
        /// <param name="singleRelations">不属于任何联通集的单一关系</param>
        private void AnalyseRelationJoin(ref List<TableSchemaList> connectRelationCollections, ref TableSchemaList singleRelations)
        {
            // 联通集编号
            //int curTag = 0;
            int curTag;
            //connectRelationCollections.Insert(curTag, new TableSchemaList());

            /**
             * 将每个Table单独编号，作为一个联通集
             */
            for (curTag = 0; curTag < tables.Count; curTag++ )
            {
                TableSchemaList tsl = new TableSchemaList();
                tables[curTag].Tag = curTag;
                tsl.Add(tables[curTag].Clone() as TableSchema);
                connectRelationCollections.Insert(curTag, tsl);
            }

            foreach (AtomCondition atom in binaryPredictions)
            {
                TableSchema leftTable = tables[atom.LeftOperand.Field.TableName.ToString()];
                TableSchema rightTable = tables[atom.RightOperand.Field.TableName.ToString()];

                if (leftTable.Tag > rightTable.Tag)
                {
                    int leftTag = leftTable.Tag;

                    connectRelationCollections[rightTable.Tag].AddRange(connectRelationCollections[leftTag]);

                    foreach (TableSchema t in connectRelationCollections[leftTag])
                        t.Tag = rightTable.Tag;

                    connectRelationCollections[leftTag].Clear();
                }
                else if (leftTable.Tag < rightTable.Tag)
                {
                    int rightTag = rightTable.Tag;

                    connectRelationCollections[leftTable.Tag].AddRange(connectRelationCollections[rightTag]);

                    foreach (TableSchema t in connectRelationCollections[rightTag])
                        t.Tag = leftTable.Tag;

                    connectRelationCollections[rightTag].Clear();
                }

                /*
                if (leftTable.Tag < 0)
                {
                    if (rightTable.Tag < 0)
                    {
                        // 左、右Table均未标记，将其加入新的联通组内
                        rightTable.Tag = curTag;
                        leftTable.Tag = curTag;

                        connectRelationCollections[curTag].Add(leftTable.Clone() as TableSchema);
                        connectRelationCollections[curTag].Add(rightTable.Clone() as TableSchema);

                        // 创建下一个备用组
                        curTag++;
                        connectRelationCollections.Insert(curTag, new TableSchemaList());
                    }
                    else
                    {
                        // 右Table被标记，左Table没有被标记过，将左边Table加入右边Table所在联通组内
                        leftTable.Tag = rightTable.Tag;

                        connectRelationCollections[rightTable.Tag].Add(leftTable.Clone() as TableSchema);
                    }

                }
                 
                else 
                {
                    if (rightTable.Tag < 0) // 左边被标记过，右边没有标记，将右边Table加入左边集合中
                    {
                        rightTable.Tag = leftTable.Tag;
                        connectRelationCollections[leftTable.Tag].Add(rightTable.Clone() as TableSchema);
                    }
                    else //左右均标记过，将编号大的联通集合合并到编号小的联通集合中
                    {
                        if (leftTable.Tag > rightTable.Tag)
                        {
                            int leftTag = leftTable.Tag;

                            connectRelationCollections[rightTable.Tag].AddRange(connectRelationCollections[leftTag]);

                            foreach (TableSchema t in connectRelationCollections[leftTag])
                                t.Tag = rightTable.Tag;

                            connectRelationCollections[leftTag].Clear();
                        }
                        else if (leftTable.Tag < rightTable.Tag)
                        {
                            int rightTag = rightTable.Tag;

                            connectRelationCollections[leftTable.Tag].AddRange(connectRelationCollections[rightTag]);

                            foreach (TableSchema t in connectRelationCollections[rightTag])
                                t.Tag = leftTable.Tag;

                            connectRelationCollections[rightTag].Clear();
                        }
                    }
                     
                }*/
            }
        
            // 消去空联通集合，将单独Table加入到singleRelations列表中并同步更联通集合内Table的Tag编号
            int pos = 0;
            while (pos < connectRelationCollections.Count)
            {
                if (connectRelationCollections[pos].Count == 0)
                {
                    connectRelationCollections.RemoveAt(pos);
                }
                else if (connectRelationCollections[pos].Count == 1)
                {
                    singleRelations.Add(connectRelationCollections[pos][0]);
                    connectRelationCollections.RemoveAt(pos);
                }
                else
                {
                    foreach (TableSchema ts in connectRelationCollections[pos])
                        ts.Tag = pos;

                    pos++;
                }
            }
        }

        /*
        private bool IsAllRelationTagged(TableSchemaList list)
        {
            foreach (TableSchema ts in list)
            {
                if (ts.Tag < 0)
                    return false;
            }

            return true;
        }
        */


    }
}
