using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Syntax;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class DisjunctiveNormalForm
    {
        public DisjunctiveNormalForm()
        {
            ConjunctiveItems = new List<ConjunctiveNormalForm>();
            PredicationItems = new List<AtomCondition>();
        }

        /// <summary>
        /// 析取范式的各项中是否含有析取范式
        ///   若没有合取范式，说明其各项为简单谓词，返回 false
        ///   否则，其中至少含有一个合取范式，返回true;
        /// </summary>
        public bool IsConjunctionIncluded
        {
            get;
            set;
        }

        public List<ConjunctiveNormalForm> ConjunctiveItems
        {
            get;
            set;
        }

        public List<AtomCondition> PredicationItems
        {
            get;
            set;
        }
    }
}
