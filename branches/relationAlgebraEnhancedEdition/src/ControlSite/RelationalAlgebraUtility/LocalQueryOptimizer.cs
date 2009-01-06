﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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


            ///////////////////////////////////////////////////////////////////////
            // 分类谓词
            // 将Where子句中的谓词分类为一元谓词和二元谓词
            // 分类结果保存unaryPredictions和binaryPredictions中
            //
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

            //////////////////////////////////////////////////////////////////////
            // Join联通分析
            // connectRelations为通过join关系联通的table
            // singleRelations为没有与其他table联通的table
            //

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
                    //System.Console.Write(r.toString());
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

                //System.Console.Write(r.toString());
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
        /// - Selection，Projection，最后和join后的结果Cart
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tree"></param>
        private void GenSingleRelationTree(TableSchema table, bool isConnected, ref Relation tree)
        {
            // 没有分片构建的关系代数树，以Project为根结点，后跟若干Selection，最后一个Selection的isDirectTableSchema为table
            Relation projectNode = new Relation();

            // table中含有最终投影属性的集合
            FieldList projectFields = new FieldList();

            // table中含有的与谓词相关的属性
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

            // 提取table相关一元谓词属性
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
            projectNode.Type = RelationalType.Projection;
            projectNode.ResultName = table.TableName.Clone() as string;

            if (!isConnected) // 非联通Table，Project主键或投影属性
            {
                if (projectFields.Count == 0)   // 无任何有效属性，投影主键
                    projectNode.RelativeAttributes.Fields.Add(table.PrimaryKeyField);
                else  // 投影属性
                    projectNode.RelativeAttributes.Fields.AddRange(projectFields);
            }
            else // 联通Table，Project投影属性和join属性
            {
                projectNode.RelativeAttributes.Fields.AddRange(joinField);
                projectNode.RelativeAttributes.Fields.AddRange(projectFields);
            }

            // 如没有Select谓词，直接在Project上挂TableSchema
            if (selectField.Count == 0)
            {
                projectNode.DirectTableSchema = table.Clone() as TableSchema;
                return;
            }

            /**
             * 存在select谓词，构建Select节点
             */
            Relation active = projectNode;

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

            System.Console.Write(projectNode.toString());

            ////////////////////////////////////////////////////////////////////////////////
            // active下开始做Localization，做完后修改active.DirectTableSchema = null;
            Relation fragmentTree = new Relation();
            LocalizeTable(table.TableName, gdd, ref fragmentTree);
            //System.Console.Write(fragmentTree.toString());

            // 遍历FragmentTree，遇到分片节点就检查tree上的Project和Select与Fragment的相容性，并将可用节点复制到FragmentTree中

            

        }

        /// <summary>
        /// 将非分片的关系代数树嵌入分片树的各个分支中
        /// </summary>
        /// <param name="fragmentTree">分片树</param>
        /// <param name="centralConditionTree">不考虑分片形成的关系代数树</param>
        /// <param name="joinAttributes">与fragment的逻辑表相关二元谓词的属性</param>
        /// <param name="selectAttributes">与fragment的逻辑表相关的一元谓词的属性</param>
        private bool AttachConditionToFragments(Relation fragmentTree, Relation centralConditionTreeRoot, FieldList joinAttributes, FieldList selectAttributes)
        {
            // 中间结点
            if (fragmentTree.Children.Count > 0)
            {
                foreach (Relation r in fragmentTree.Children)
                {
                    if (!AttachConditionToFragments(r, centralConditionTreeRoot, joinAttributes, selectAttributes))
                        return false;
                }

                return true;
            }

            ////////////////////////////////////////////////////////////////
            // 分片结点
            //

            // 分片的划分条件
            List<AtomCondition> fragmentCondition = new List<AtomCondition>();

            // 本fragmentTree结点对应的分片对象
            Fragment fragment = gdd.Fragments.GetFragmentByName(fragmentTree.DirectTableSchema.TableName);

            // 分片的必要属性（不知是否有用）
            FieldList fragmentNecessaryAttributes = new FieldList();
            foreach (Field f in fragment.Schema.Fields)
            {
                if ((joinAttributes[f] != null || selectAttributes[f] != null ) && fragmentNecessaryAttributes[f] == null)
                    fragmentNecessaryAttributes.Add(f);
            }

            Debug.Assert(fragmentNecessaryAttributes.Count > 0);

            // 收集分片的所有分片条件
            Fragment traveller = fragment;
            while ( traveller != null)
            {
                if (traveller.Condition != null)
                {
                    ConditionConverter conditionConverter = new ConditionConverter();
                    conditionConverter.Convert(traveller.Condition, NormalFormType.Conjunction);
                    ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;

                    foreach (AtomCondition atom in normalForm.PredicationItems)
                        fragmentCondition.Add(atom.Clone() as AtomCondition);
                }

                traveller = traveller.Parent;
            }

            // 合并叶节点和分支
            Relation centralTreeNode = centralConditionTreeRoot;
            Relation growPoint = fragmentTree;
            while (centralTreeNode != null)
            {

                if (centralTreeNode.Type == RelationalType.Projection)  // 根据fragment的必要属性创建Project结点
                {
                    Relation projection = new Relation();
                    projection.Type = RelationalType.Projection;

                    projection.RelativeAttributes.Fields.AddRange(fragmentNecessaryAttributes);

                    growPoint.LeftRelation = projection;
                }
                else if (centralTreeNode.Type == RelationalType.Selection)  // 检查Selet条件
                {
                    // 检查Select条件与分片条件是否冲突
                    // 冲突：本分支返回false 
                    

                    // 不冲突：继续生长

                }
                else
                {
                    Debug.Assert(false);                 
                }

                
            }

            return true;
        }


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

        /// <summary>
        /// 将Decomposition后的关系代数树的叶节点（Selection）转换为Fragment组成的关系代数子树
        /// </summary>
        /// <param name="selection">经Decomposition操作后的关系代数树的叶节点</param>
        /// <param name="gdd">GDD</param>
        /// <returns>Error code</returns>
        private int LocalizeTable(string tableName, GlobalDirectory gdd, ref Relation root)
        {

            // 在gdd中查找与逻辑表对应的分片
            Fragment fragments = null;
            foreach (Fragment f in gdd.Fragments)
            {
                if (f.LogicSchema.TableName == tableName)
                {
                    fragments = f;
                    break;
                }
            }

            if (fragments == null)
                return -1;

            // 根据分片构造关系代数树
            ReconstructFragment(fragments, ref root);

            return 0;
        }

        /// <summary>
        /// 根据分片构造关系代数树
        /// </summary>
        /// <param name="f">一个分片模式</param>
        /// <param name="parent"></param>
        private void ReconstructFragment(Fragment f, ref Relation root)
        {
            if (f.Children.Count == 0)
                return;

            //parent.DirectTableSchema = null;

            // 水平划分，Union连接
            if (f.Children[0].Type == FragmentType.Horizontal)
            {
                Relation union = new Relation();
                union.Type = RelationalType.Union;
                root.LeftRelation = union;

                foreach (Fragment subf in f.Children)
                {
                    Relation selection = new Relation();
                    selection.Type = RelationalType.Selection;
                    selection.DirectTableSchema = subf.Schema.Clone() as TableSchema;
                    selection.DirectTableSchema.ReplaceTableName(subf.Name);
                    selection.DirectTableSchema.IsAllFields = true;

                    union.Children.Add(selection);

                    ReconstructFragment(subf, ref selection);
                }
            }
            else if (f.Children[0].Type == FragmentType.Vertical) // 垂直划分, join连接
            {
                Relation activeRelation = null;
                foreach (Fragment subf in f.Children)
                {
                    Relation selection = new Relation();
                    selection.Type = RelationalType.Selection;
                    selection.DirectTableSchema = subf.Schema.Clone() as TableSchema;
                    selection.DirectTableSchema.ReplaceTableName(subf.Name);
                    //selection.RelativeAttributes = subf.Schema; 

                    if (activeRelation == null)
                    {
                        activeRelation = selection;
                    }
                    else
                    {
                        Relation join = new Relation();
                        join.Type = RelationalType.Join;
                        join.LeftRelation = activeRelation;
                        join.RightRelation = selection;

                        // join条件
                        Field field1 = join.LeftRelation.DirectTableSchema.PrimaryKeyField.Clone() as Field;
                        field1.TableName = join.LeftRelation.DirectTableSchema.TableName;

                        join.RelativeAttributes.Fields.Add(field1);

                        Field field2 = join.RightRelation.DirectTableSchema.PrimaryKeyField.Clone() as Field;
                        field2.TableName = join.RightRelation.DirectTableSchema.TableName;

                        join.RelativeAttributes.Fields.Add(field2);

                        activeRelation = join;
                    }

                    ReconstructFragment(subf, ref selection);
                }
                root.LeftRelation = activeRelation;
            }
        }


    }
}
