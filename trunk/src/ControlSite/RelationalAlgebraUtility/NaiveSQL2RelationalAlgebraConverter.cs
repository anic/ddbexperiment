using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.SQLSyntax.Operation;

namespace DistDBMS.ControlSite.RelationalAlgebraUtility
{
    class NaiveSQL2RelationalAlgebraConverter:SQL2RelationalAlgebraInterface
    {
        private enum QueryCalculusType
        {
            Selection,
            Delete,
            Insertion
        }

        private QueryCalculusType queryType;

        public GlobalDirectory dictionary;

        public DistDBMS.ControlSite.SQLSyntax.Operation.Selection selectionCalculus = null;

        private DistDBMS.ControlSite.SQLSyntax.Operation.Deletion deleteCalculus = null;

        private DistDBMS.ControlSite.SQLSyntax.Operation.Insertion insertionCaculus = null;

        private RelationAlgebraConvertError error;
        
        public Relation relationAlgebra;


        void SQL2RelationalAlgebraInterface.SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Selection queryCalculus)
        {
            queryType = QueryCalculusType.Selection;
            selectionCalculus = queryCalculus;
        }

        void SQL2RelationalAlgebraInterface.SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Deletion queryCalculus)
        {
            queryType = QueryCalculusType.Delete;
            deleteCalculus = queryCalculus;
        }

        void SQL2RelationalAlgebraInterface.SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Insertion queryCalculus)
        {
            queryType = QueryCalculusType.Insertion;
            insertionCaculus = queryCalculus;
        }

        Relation SQL2RelationalAlgebraInterface.SQL2RelationalAlgebra(GlobalDirectory gdd)
        {
            dictionary = gdd;

            Relation decomposedQuery = QueryDecomposition();

            QueryLocalization(decomposedQuery, dictionary);

            return decomposedQuery;
        }

        RelationAlgebraConvertError SQL2RelationalAlgebraInterface.GetLastError()
        {
            return error;
        }


        /// <summary>
        /// 将SQL语法树转换为关系代数树，不考虑分片
        /// </summary>
        /// <returns>关系代数树根节点</returns>
        private Relation QueryDecomposition()
        {
            Relation result = null;

            if (queryType == QueryCalculusType.Selection)
            {
                result = SelectionDecomposition();
            }
            else if (queryType == QueryCalculusType.Insertion)
            {
                result = InsertionDecomposition();
            }
            else if (queryType == QueryCalculusType.Delete)
            {
                result = DeleteDecomposition();
            }
            else
            {
                error.ErrorMsg = "无法识别的操作类型";
            }

            return result;

        }


        /// <summary>
        /// 将数据源域转换为关系代数树
        /// 
        /// 按顺序将每个表用CartesianProduct连接，不考虑join条件和计算顺序。
        /// 除了叶节点，每个CartesianProduct节点的LeftChild为上一步CartesianProduct产生的中间数据，
        /// RightChild为经过全选择(Select * From [one table])产生的单表数据
        /// 
        /// 叶节点的LeftChild, RightChild均为经过全选择(Select * From [one table])产生的单表数据
        /// </summary>
        /// <param name="sources">Parser产生的数据源列表，对应From...Where之间内容</param>
        /// <returns>所有表使用CartesianProduct连接产生的关系代数树的根结点</returns>
        private Relation ConvertSource(List<TableSchema> sources)
        {
            Boolean skipFirstElement = false;
            Relation activeRelation = null;

            foreach (TableSchema tableSchema in sources)
            {

                // 为每个关系添加一个辅助查询 Select * From [tableSchema]
                // 在优化后可以与其他条件合并
                Relation selection = new Relation();
                selection.Type = RelationalType.Selection;
                selection.IsDirectTableSchema = true;
                selection.DirectTableSchema = tableSchema;

                if (!skipFirstElement)
                {
                    skipFirstElement = true;
                    activeRelation = selection;
                    continue;
                }

                // 从第2个Table开始，与之前结果进行 x 运算
                Relation cartesianProduct = new Relation();
                cartesianProduct.Type = RelationalType.CartesianProduct;
                cartesianProduct.IsDirectTableSchema = false;
                cartesianProduct.Children.Add(activeRelation);
                cartesianProduct.Children.Add(selection);

                activeRelation = cartesianProduct;
            }

            return activeRelation;
        }


        /// <summary>
        /// 将条件域转换为关系代数树结构
        /// </summary>
        /// <param name="root">经过ConvertSource转换过的关系代数树结构的根结点</param>
        /// <param name="predications">Select或Delete命令中的条件域(由Parser生成的Condition)</param>
        /// <returns>添加Condition条件后的RelationalAlgebra树结构</returns>
        private Relation ConvertCondition(Relation root, Condition predications)
        {
            if (predications == null)
                return root;

            Relation activeRelation = root;

            ConditionConverter conditionConverter = new ConditionConverter();
            conditionConverter.Convert(predications, NormalFormType.Conjunction);
            ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;

            foreach (AtomCondition atomPredication in normalForm.PredicationItems)
            {
                Relation selection = new Relation();
                selection.Type = RelationalType.Selection;
                selection.Children.Add(activeRelation);

                Condition predication = new Condition();                
                predication.AtomCondition = atomPredication;

                selection.IsDirectTableSchema = false;
                selection.Predication = predication;

                activeRelation = selection;
            }

            return activeRelation;
        }


        /// <summary>
        /// 将投影域(select命令中Select与From之间部分)转换为关系代数
        /// </summary>
        /// <param name="root"> 经过ConvertSource和ConvertCondition转换过的关系代数树结构的根结点</param>
        /// <param name="schema"> 经Parse生成的投影域(Select ... From)</param>
        /// <returns>以Projection为根的RelationalAlgebra树结构</returns>
        private Relation ConvertField(Relation root, TableSchema schema)
        { 
            Relation projection = new Relation();
            projection.Type = RelationalType.Projection;
            projection.IsDirectTableSchema = false;
            projection.Children.Add(root);

            projection.RelativeAttributes = schema;

            return projection;
        }


        /// <summary>
        /// 使用Fragment数据和Union操作替换关系代数树中的叶节点
        /// </summary>
        private void QueryLocalization(Relation decompositedQuery, GlobalDirectory gdd)
        {
            // Selection 叶节点，在gdd中查找该逻辑表对应分片方案
            if (decompositedQuery.Children.Count == 0 && decompositedQuery.IsDirectTableSchema)
            {
                LocalizeTable(ref decompositedQuery, gdd);
            }
            else // 中间节点
            {
                foreach (Relation r in decompositedQuery.Children)
                {
                    QueryLocalization(r, gdd);
                }
            }
        }
        
        /// <summary>
        /// 将Decomposition后的关系代数树的叶节点（Selection）转换为Fragment组成的关系代数子树
        /// </summary>
        /// <param name="selection">经Decomposition操作后的关系代数树的叶节点</param>
        /// <param name="gdd">GDD</param>
        /// <returns>Error code</returns>
        private int LocalizeTable(ref Relation selection, GlobalDirectory gdd)
        {

            // 在gdd中查找与逻辑表对应的分片
            Fragment fragments = null;
            foreach (Fragment f in gdd.Fragments)
            {
                if (f.LogicTable.TableName == selection.DirectTableSchema.TableName)
                {
                    fragments = f;
                    break;
                }
            }

            if (fragments == null)
                return -1;

            // 根据分片构造关系代数树
            ReconstructFragment(fragments, ref selection);
            return 0;
        }

        /// <summary>
        /// 根据分片构造关系代数树
        /// </summary>
        /// <param name="f">一个分片模式</param>
        /// <param name="parent"></param>
        private void ReconstructFragment(Fragment f, ref Relation parent)
        {
            if (f.Children.Count == 0)
            {
                parent.IsDirectTableSchema = true;
                return;
            }

            parent.IsDirectTableSchema = false;

            // 水平划分，Union连接
            if (f.Children[0].Type == FragmentType.Horizontal)
            {
                Relation union = new Relation();
                union.Type = RelationalType.Union;
                parent.LeftRelation = union;

                foreach (Fragment subf in f.Children)
                {
                    Relation selection = new Relation();
                    selection.Type = RelationalType.Selection;
                    selection.DirectTableSchema = new TableSchema();
                    selection.DirectTableSchema.TableName = subf.Name;
                    selection.DirectTableSchema.IsAllFields = true;
                    selection.Predication = subf.Condition;
                    selection.RelativeAttributes = subf.Schema;

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
                    //selection.DirectTableSchema = new TableSchema();
                    //selection.DirectTableSchema.TableName = subf.Name;
                    selection.DirectTableSchema = subf.Schema;
                    selection.DirectTableSchema.TableName = subf.Name;
                    //selection.DirectTableSchema.IsAllFields = subf.Schema.IsAllFields; //??是否这样使用IsAllField?
                    selection.RelativeAttributes = subf.Schema;

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
                parent.LeftRelation = activeRelation;
            }
        }

        /// <summary>
        /// 将Select命令的SQL语法树转换为关系代数树，不考虑分片
        /// </summary>
        /// <returns></returns>
        private Relation SelectionDecomposition()
        {
            // From ...
            Relation result = ConvertSource(selectionCalculus.Sources);

            // From ... Where ...
            result = ConvertCondition(result, selectionCalculus.Condition);

            // Select ...From ... Where ...
            result = ConvertField(result, selectionCalculus.Fields);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Relation DeleteDecomposition()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Relation InsertionDecomposition()
        {
            return null;
        }
    }
}
