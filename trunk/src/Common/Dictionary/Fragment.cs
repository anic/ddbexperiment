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

    public class Fragment
    {
        /// <summary>
        /// 分配的站点
        /// </summary>
        public Site Site { get; set; }

        /// <summary>
        /// 原始的逻辑表格
        /// </summary>
        public TableSchema LogicTable { get; set; }

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
        /// 分片后最终的样式表
        /// </summary>
        public TableSchema Schema {
            get
            {
                //垂直分片或者顶层分片
                if (Type == FragmentType.Vertical || Type == FragmentType.None)
                    return ts;
                else if (Parent!=null) //水平分片
                    return Parent.Schema;
                else 
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
            LogicTable = null;
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
