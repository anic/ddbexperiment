using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Syntax;
using DistDBMS.Common.RelationalAlgebra.Entity;

namespace DistDBMS.ControlSite.RelationalAlgebraUtility
{
    class ConditionConverter
    {

        private NormalFormType normalFormType;

        private Condition condition;

        public void Convert(Condition rawCondition, NormalFormType type)
        {
            normalFormType = type;
            condition = rawCondition;

            if (normalFormType == NormalFormType.Conjunction)
            {
                ConjunctiveNormalize();
            }
            else if (normalFormType == NormalFormType.Disjunction)
            {
                DisjunctiveNormalize();
            }
        }
        
        public ConjunctiveNormalForm ConjunctionNormalForm
        {
            get
            {
                if (normalFormType != NormalFormType.Conjunction)
                    return null;

                return conjunctionNormalForm;
            }
        }

        public DisjunctiveNormalForm DisjunctionNormalForm
        {
            get
            {
                if (normalFormType != NormalFormType.Disjunction)
                    return null;

                return disjunctionNormalForm;
            }
        }

        private ConjunctiveNormalForm conjunctionNormalForm = null;

        private DisjunctiveNormalForm disjunctionNormalForm = null;

        /// <summary>
        /// 将Condition转换为合取范式
        /// 
        /// 简单实现，目前只处理条件中只有AND的情况
        /// </summary>
        private void ConjunctiveNormalize()
        { 
            if (condition == null)
                return;

            conjunctionNormalForm = new ConjunctiveNormalForm();
            conjunctionNormalForm.IsDisjunctionIncluded = false;

            Condition activeCondition = condition;

            while (!activeCondition.IsAtomCondition)
            {
                conjunctionNormalForm.PredicationItems.Add(activeCondition.RightCondition.AtomCondition);
                activeCondition = activeCondition.LeftCondition;
            }
            conjunctionNormalForm.PredicationItems.Add(activeCondition.AtomCondition);
        }


        /// <summary>
        /// 将Condition转换为析取范式
        /// </summary>
        private void DisjunctiveNormalize()
        {
        }

    }
}
