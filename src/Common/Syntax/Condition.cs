using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common;

namespace DistDBMS.Common.Syntax
{
    [Serializable]
    public class Condition:ICloneable
    {
        /// <summary>
        /// 连接符，And / Or
        /// </summary>
        public RelationOperator Operator{get;set;}

        /// <summary>
        /// 左条件
        /// </summary>
        public Condition LeftCondition { get; set; }

        /// <summary>
        /// 右条件
        /// </summary>
        public Condition RightCondition { get; set; }

        /// <summary>
        /// 是否原子条件，即是否有连接符
        /// </summary>
        public bool IsAtomCondition { get { return (AtomCondition != null); } }

        /// <summary>
        /// 原子条件
        /// </summary>
        public AtomCondition AtomCondition { get; set; }

        //判断条件是否为空的条件
        public bool IsEmpty {
            get
            {
                return (!IsAtomCondition && (LeftCondition == null || RightCondition == null));
            }
        }

        public Condition()
        {
            Operator = RelationOperator.And;
            LeftCondition = null;
            RightCondition = null;
            AtomCondition = null;
            
        }

        public new string ToString()
        {
            if (IsAtomCondition)
                return AtomCondition.ToString();
            else if (LeftCondition != null && RightCondition != null)
                return LeftCondition.ToString() + " " + Operator.ToString() + " " + RightCondition.ToString();
            else
                return "Empty";
        }


        #region ICloneable Members

        public object Clone()
        {
            Condition result = new Condition();
            result.Operator = Operator;
            if (AtomCondition != null)
                result.AtomCondition = AtomCondition.Clone() as AtomCondition;

            if (LeftCondition != null)
                result.LeftCondition = LeftCondition.Clone() as Condition;

            if (RightCondition != null)
                result.RightCondition = RightCondition.Clone() as Condition;

            return result;
        }

        #endregion
    }
}
