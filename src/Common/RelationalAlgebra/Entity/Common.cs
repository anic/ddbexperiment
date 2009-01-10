using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    /// <summary>
    /// 关系代数类型
    /// </summary>
    public enum RelationalType
    { 
        Projection,
        Selection,
        CartesianProduct,
        Union,
        Difference,
        Intersection,
        Join,
        Semijoin,
        
    }


    public enum NormalFormType
    {
        Conjunction,
        Disjunction
    }

}
