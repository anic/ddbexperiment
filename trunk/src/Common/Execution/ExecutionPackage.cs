using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Execution
{
    public class ExecutionPackage
    {
        public string ID { get; set; }

        public enum PackageType
        { 
            Plan,
            Data
        }

        public PackageType Type { get; set; }

        public ExecutionPackage()
        {
            ID = "";
            Type = PackageType.Data;
            Object = null;
        }

        /// <summary>
        /// 根据Type来判断对象的类型
        /// 如果Type == Plan，则包含了执行计划；否则包含Table
        /// </summary>
        public object Object { get; set; }
    }
}
