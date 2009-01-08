using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Table;

namespace DistDBMS.Common.Dictionary
{
    public enum FragmentType
    {
        /// <summary>
        /// 分片，无条件
        /// </summary>
        None,
        /// <summary>
        /// 水平分片
        /// </summary>
        Horizontal, 
        /// <summary>
        /// 垂直分片
        /// </summary>
        Vertical
    }

    /// <summary>
    /// 分片定义
    /// </summary>
    [Serializable]
    public class Fragment
    {
        /// <summary>
        /// 分配的站点
        /// </summary>
        public Site Site { get; set; }

        /// <summary>
        /// 原始的逻辑表格
        /// </summary>
        public TableSchema LogicSchema { get; set; }

        /// <summary>
        /// 划分条件,可以为无
        /// </summary>
        public Condition Condition { get; set; }

        ///// <summary>
        ///// 如果是垂直分片则记录分片的表样式
        ///// </summary>
        //public TableSchemeList ConditionSchemes { get { return conSchemes; } }
        //TableSchemeList conSchemes;

        /// <summary>
        /// 上级的分片
        /// </summary>
        public Fragment Parent { get; set; }

        /// <summary>
        /// 下级分片
        /// </summary>
        public List<Fragment> Children { get { return children; } }
        List<Fragment> children;

        /// <summary>
        /// 分片方式
        /// </summary>
        public FragmentType Type { get; set; }

        /// <summary>
        /// 该分片是否被垂直划分过
        /// 即，该分片是否是垂直划分或属于一个垂直划分的子划分
        /// </summary>
        public bool HasVerticalDivided 
        { 
            get 
            {
                Fragment f = this;
                do
                {
                    if (f.Type == FragmentType.Vertical)
                        return true;

                    f = f.Parent;
                }
                while (f != null);

                return false;
            } 
        }

        /// <summary>
        /// 分片后最终的样式表,tablename将会换成Name
        /// </summary>
        public TableSchema Schema {
            get
            {
                
                switch(Type)
                {
                    case FragmentType.None: //顶层分片
                        return ts.Clone() as TableSchema;
                    case FragmentType.Vertical: //垂直分片
                        {
                            TableSchema result = ts.Clone() as TableSchema;
                            TableSchema logicSchema = LogicSchema;
                            for (int i = 0; i < result.Fields.Count; i++)
                            {
                                Field resultField = result.Fields[i];
                                Field logicField = logicSchema[resultField.AttributeName];
                                result.Fields[i] = logicField.Clone() as Field;
                            }
                            result.ReplaceTableName(Name);
                            return result;
                        }
                    case FragmentType.Horizontal://水平分片
                        {
                            TableSchema result = Parent.Schema.Clone() as TableSchema;
                            result.ReplaceTableName(Name);
                            return result;
                        }
                }
                return null;
            }
            set
            {
                ts = value;
            }
        }
        TableSchema ts;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public Fragment()
        {
            children = new List<Fragment>();
            Site = null;
            LogicSchema = null;
            Condition = null;
            Parent = null;
            Type = FragmentType.Horizontal;
            ts = null;
            Name = "";
        }

        public new string ToString()
        {
            return Name;
        }
    }
}
