using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;

using DistDBMS.Common.Table;


namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class Relation
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
        /// 是否关系直接的表
        /// </summary>
        public bool IsDirectTableSchema { get; set; }

        /// <summary>
        /// 如果关系直接的表，则获得原子的表
        /// </summary>
        public TableSchema DirectTableSchema { get; set; }

        /// <summary>
        /// 谓词，如果有
        /// </summary>
        public Predication Predication { get; set; }

        /// <summary>
        /// 相关的属性集
        /// </summary>
        public TableSchema RelativeAttributes { get; set; }

        public TableSchema ResultSchema
        {
            get
            { 
                //TODO:未完成
                TableSchema result = new TableSchema();
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
            IsDirectTableSchema = false;
            DirectTableSchema = new TableSchema();
            RelativeAttributes = new TableSchema();
            Predication = new Predication();
            Content = "";
        }

        public static Relation EmptyRelation
        {
            get { return null; }
        }

        

    }
}
