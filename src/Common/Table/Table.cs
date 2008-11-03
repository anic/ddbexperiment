using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DistDBMS.Common.Table
{
    /// <summary>
    /// 一个包含数据的表
    /// </summary>
    public class Table:ISerializable
    {
        /// <summary>
        /// 表头，记录了表名、属性名、属性的类型（int）
        /// </summary>
        public TableScheme Scheme { get; set; }

        /// <summary>
        /// 元组列表
        /// </summary>
        public List<Tuple> Tuples { get; set; }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
