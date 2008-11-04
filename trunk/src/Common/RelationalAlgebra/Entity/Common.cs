using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public enum RelationalType
    { 
        Projection,
        Selection,
        CartesianProduct,
        Union,
        Difference,
        Intersection,
        Join,
        Semijoin
    }
}
