using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Dictionary;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;

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

        public LocalQueryOptimizer(Relation tree, List<TableSchema> source, GlobalDirectory gdd)
        {
            queryTree = tree;
            this.gdd = gdd;

            unaryPredictions = new List<AtomCondition>();
            binaryPredictions = new List<AtomCondition>();
            fragments = new List<Fragment>();
            tables = new TableSchemaList();
            tables.AddRange(source);

            // 分类谓词
            ClassifyPrediction();

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

            System.Console.WriteLine("************Connect Group**************");
            for (int g = 0; g < connectRelations.Count; g++)
            {
                System.Console.WriteLine("Connect gourp[" + g.ToString() + "]:");
                foreach (TableSchema ts in connectRelations[g])
                {
                    System.Console.Write(ts.Tag+"_"+ts.TableName + " ");
                }
                System.Console.WriteLine();
            }

            System.Console.WriteLine("*********** Single Relation **********");
            foreach (TableSchema ts in singleRelations)
            {
                System.Console.Write(ts.Tag + "_" + ts.TableName + " ");
            }
            System.Console.WriteLine();

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
            int curTag = 0;
            connectRelationCollections.Insert(curTag, new TableSchemaList());
            
            foreach (AtomCondition atom in binaryPredictions)
            {
                TableSchema leftTable = tables[atom.LeftOperand.Field.TableName.ToString()];
                TableSchema rightTable = tables[atom.RightOperand.Field.TableName.ToString()];

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
                     
                }
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
