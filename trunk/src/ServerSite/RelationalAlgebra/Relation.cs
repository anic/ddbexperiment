using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.SQLSyntax;

namespace DistDBMS.ServerSite.RelationalAlgebra
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
        public bool IsAtomRealtion { get; set; }

        /// <summary>
        /// 如果是原子关系，则获得原子的表
        /// </summary>
        public Table AtomTable { get; set; }

        /// <summary>
        /// 谓词，如果有
        /// </summary>
        public Predication Predication { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public String Content { get; set; }

        public Relation()
        {
            
            LeftRelation = null;
            RightRelation = null;
            IsAtomRealtion = false;
            AtomTable = new Table();
            Predication = new Predication();
            Content = "";
        }

        public static Relation EmptyRelation
        {
            get { return null; }
        }

        

    }
}
