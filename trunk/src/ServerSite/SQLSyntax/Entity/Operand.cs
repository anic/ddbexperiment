using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class Operand
    {
        static Exception InvalidValueConvertion = new Exception("InvalidValueConvertion");

        /// <summary>
        /// 是否是值
        /// </summary>
        public bool IsValue { get; set; }

        /// <summary>
        /// 如果是值，其类型，如Int,Double等
        /// </summary>
        public AttributeType ValueType { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 转化成Boolean值
        /// </summary>
        public bool ToBooleanValue { 
            get{
                if (IsValue)
                    return Convert.ToBoolean(Value);

                throw InvalidValueConvertion;
            } 
        }

        /// <summary>
        /// 转化成Int值
        /// </summary>
        public int ToIntValue {
            get
            {
                if (IsValue)
                    return Convert.ToInt32(Value);

                throw InvalidValueConvertion;
            }
        }

        public double ToDoubleValue
        {
            get
            {
                if (IsValue)
                    return Convert.ToDouble(Value);

                throw InvalidValueConvertion;
            }
        }

        public string ToStringValue
        {
            get
            {
                if (IsValue)
                    return Convert.ToString(Value);

                throw InvalidValueConvertion;
            }
        }

        public DateTime ToDateValue
        {
            get
            {
                if (IsValue)
                    return Convert.ToDateTime(Value);

                throw InvalidValueConvertion;
            }
        }

        /// <summary>
        /// 如果不是值，是某个表的某个属性
        /// </summary>
        public Field Field { get; set; }

    }
}
