using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using DistDBMS.Common.Syntax;

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
        public TableSchemaList Schemas { get {return schemas; } }
        TableSchemaList schemas;

        /// <summary>
        /// 分片列表
        /// </summary>
        public FragmentList Fragments { get { return fragments; } }
        FragmentList fragments;
        
        //统计信息怎么加？

        public GlobalDataDictionary()
        {
            sites = new SiteList();
            schemas = new TableSchemaList();
            fragments = new FragmentList();
        }

        


        
    }
}
