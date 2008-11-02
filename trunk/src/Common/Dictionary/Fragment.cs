using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.Entity;

namespace DistDBMS.Common.Dictionary
{
    public enum FragmentType
    {
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
        /// 数据库中的原始表格
        /// </summary>
        public TableScheme DbTable { get; set; }

        /// <summary>
        /// 水平划分条件,可以为无
        /// </summary>
        public Condition Condition { get; set; }

        /// <summary>
        /// 如果是垂直分片则记录分片的表样式
        /// </summary>
        public TableSchemeList ConditionSchemes { get { return conSchemes; } }
        TableSchemeList conSchemes;

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
        public TableScheme Scheme { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        
        
        public Fragment()
        {
            children = new List<Fragment>();
            Site = null;
            DbTable = null;
            Condition = null;
            Parent = null;
            Type = FragmentType.Horizontal;
            conSchemes = new TableSchemeList();
            Scheme = null;
            Name = "";
        }
    }
}
