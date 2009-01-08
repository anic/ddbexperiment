using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite.RelationalAlgebraUtility
{
    interface SQL2RelationalAlgebraInterface
    {
        /// <summary>
        /// Handle 'select' synatx tree
        /// </summary>
        /// <param name="queryCalculus"></param>
        void SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Selection queryCalculus);

        /// <summary>
        /// Handle 'insert' syntax tree
        /// </summary>
        /// <param name="queryCalculus"></param>
        void SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Insertion queryCalculus);

        /// <summary>
        /// Handle 'delete' syntax tree
        /// </summary>
        /// <param name="queryCalculus"></param>
        void SetQueryCalculus(DistDBMS.ControlSite.SQLSyntax.Operation.Deletion queryCalculus);

        /// <summary>
        /// Convert SQL syntax tree to Relational algebra
        /// </summary>
        /// <returns>Relational Algebra</returns>
        Relation SQL2RelationalAlgebra(GlobalDirectory gdd, bool isOptimize);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        RelationAlgebraConvertError GetLastError();

    }
}
