using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax;

using DistDBMS.Common.Entity;
using DistDBMS.ServerSite.Common;

namespace DistDBMS.ServerSite.RelationalAlgebra.Entity
{
    class Relation
    {
        /// <summary>
        /// 关系代数的关系类型，如Selection,Projection
        /// </summary>
        public RelationalType Type { get; set; }

        /// <summary>
        /// 左关系
        /// </summary>
        public Relation LeftRelation { get; set; }

        /// <summary>
        /// 右关系
        /// </summary>
        public Relation RightRelation { get; set; }

        /// <summary>
        /// 是否原子关系，不能再细分
        /// </summary>
        public bool IsDirectTableScheme { get; set; }

        /// <summary>
        /// 如果是原子关系，则获得原子的表
        /// </summary>
        public TableScheme DirectTableScheme { get; set; }

        /// <summary>
        /// 谓词，如果有
        /// </summary>
        public Predication Predication { get; set; }

        /// <summary>
        /// 相关的属性集
        /// </summary>
        public TableScheme RelativeAttributes { get; set; }

        public TableScheme ResultScheme
        {
            get
            { 
                //TODO:未完成
                TableScheme result = new TableScheme();
                return result;
            }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public Relation()
        {
            
            LeftRelation = null;
            RightRelation = null;
            IsDirectTableScheme = false;
            DirectTableScheme = new TableScheme();
            RelativeAttributes = new TableScheme();
            Predication = new Predication();
            Content = "";
        }

        public static Relation EmptyRelation
        {
            get { return null; }
        }

        

    }
}
