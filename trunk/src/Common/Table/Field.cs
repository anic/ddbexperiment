using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    /// <summary>
    /// 表中的域
    /// </summary>
    public class Field:ICloneable
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 所属的表表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 属性字段的类型
        /// </summary>
        public AttributeType AttributeType { get; set; }

        /// <summary>
        /// 如Int32后面的32可以存入，可能没用，暂时做着
        /// </summary>
        public object Addition { get; set; }

        /// <summary>
        /// 表中是否以这个属性作为索引
        /// </summary>
        public bool Indexed { get; set; }

        /// <summary>
        /// 表中是否以这个属性作为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        

        public Field()
        {
            AttributeName = "";
            TableName = "";
            Addition = null;
            Indexed = false;
            IsPrimaryKey = false;
            AttributeType = AttributeType.Unknown;
        }

        public new string ToString()
        {
            return TableName + "." + AttributeName;
        }



        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
