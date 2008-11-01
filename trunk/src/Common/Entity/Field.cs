using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Entity
{
    /// <summary>
    /// 表中的域
    /// </summary>
    public class Field
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
        /// 属性对应的表
        /// </summary>
        public Table Table
        {
            get;
            set;
        }

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public Field()
        {
            AttributeName = "UnSet";
            AttributeType = AttributeType.Unkown;
        }
    }
}
