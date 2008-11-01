﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.RelationalAlgebra
{
    enum RelationalType
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
