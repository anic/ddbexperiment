using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Syntax;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class ConjunctiveNormalForm
    {
        public ConjunctiveNormalForm()
        {
            DisjunctiveItems = new List<DisjunctiveNormalForm>();
            PredicationItems = new List<AtomCondition>();
        }

        /// <summary>
        /// 合取范式的各项中是否含有析取范式
        ///   若没有析取范式，说明其各项为简单谓词，返回 false
        ///   否则，其中至少含有一个析取范式，返回true;
        /// </summary>
        public bool IsDisjunctionIncluded
        {
            get;
            set;
        }

        public List<DisjunctiveNormalForm> DisjunctiveItems
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
