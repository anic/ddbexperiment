using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    /// <summary>
    /// 全局数据字典
    /// </summary>
    public class GlobalDataDictionary
    {
        /// <summary>
        /// 站点列表
        /// </summary>
        public SiteList Sites { get { return sites; } }
        SiteList sites;

        /// <summary>
        /// 表列表
        /// </summary>
        public TableSchemeList Schemes { get {return schemes; } }
        TableSchemeList schemes;

        /// <summary>
        /// 分片列表
        /// </summary>
        public FragmentList Fragments { get { return fragments; } }
        FragmentList fragments;
        
        //统计信息怎么加？

        public GlobalDataDictionary()
        {
            sites = new SiteList();
            schemes = new TableSchemeList();
            fragments = new FragmentList();
        }
    }
}
