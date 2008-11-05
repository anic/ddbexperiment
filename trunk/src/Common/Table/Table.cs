using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DistDBMS.Common.Table
{
    /// <summary>
    /// 一个包含数据的表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 表头，记录了表名、属性名、属性的类型（int）
        /// </summary>
        public TableSchema Schema { get; set; }

        /// <summary>
        /// 元组列表
        /// </summary>
        public List<Tuple> Tuples { get; set; }

     
    }
}
