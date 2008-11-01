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
        /// 是否表示某个表的属性
        /// </summary>
        public bool IsField
        {
            get { return !IsValue; }
        }

        /// <summary>
        /// 如果不是值，是某个表的某个属性
        /// </summary>
        public Field Field { get; set; }

        /// <summary>
        /// 如果是值，其类型，如Int,Double等
        /// </summary>
        public AttributeType ValueType { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }


        public Operand()
        {
            IsValue = true;
            Field = new Field();
            ValueType = AttributeType.Unknown;
        }

        public new string ToString()
        {
            if (IsValue)
                return Value.ToString();
            else
            {
                return Field.TableName + "." + Field.AttributeName;
            }
        }

        #region 将值转化为对应类型的方法
        /// <summary>
        /// 转化成Boolean值
        /// </summary>
        /// <remarks>将Value转化为Bool类型</remarks>
        /// <exception cref="Exception">如果不是值类型，调用此方法会抛出异常</exception>
        public bool ToBooleanValue()
        {
            if (IsValue)
                return Convert.ToBoolean(Value);

            throw InvalidValueConvertion;
        }

        /// <summary>
        /// 转化成Int值
        /// </summary>
        public int ToIntValue()
        {
            if (IsValue)
                return Convert.ToInt32(Value);

            throw InvalidValueConvertion;
        }

        /// <summary>
        /// 转化成Double值
        /// </summary>
        /// <returns></returns>
        public double ToDoubleValue()
        {
            if (IsValue)
                return Convert.ToDouble(Value);

            throw InvalidValueConvertion;

        }

        public string ToStringValue()
        {
            if (IsValue)
                return Convert.ToString(Value);

            throw InvalidValueConvertion;

        }

        public DateTime ToDateTimeValue()
        {
            if (IsValue)
                return Convert.ToDateTime(Value);

            throw InvalidValueConvertion;

        }

        #endregion

    }
}
