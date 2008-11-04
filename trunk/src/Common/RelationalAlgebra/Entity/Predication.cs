using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
    public class Predication
    {
        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public Predication()
        {
            Content = "";
        }
    }
}
