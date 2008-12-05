using System;
using System.Collections.Generic;
using System.Text;


namespace DistDBMS.Common.Execution
{
    public class ExecutionPackage 
    {
        public int ID { get; set; }

        public enum PackageType
        { 
            Plan = 0,
            Data,
            GDDInit
        }

        public PackageType Type { get; set; }

        public ExecutionPackage()
        {
            ID = -1;
            Type = PackageType.Data;
            Object = null;
        }

        /// <summary>
        /// 根据Type来判断对象的类型
        /// 如果Type == Plan，则包含了执行计划；否则包含Table
        /// </summary>
        public object Object { get; set; }

        public new string ToString()
        {
            return ID.ToString();
        }
    }
}
