using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    /// <summary>
    /// 站点定义
    /// </summary>
    [Serializable]
    public class Site
    {

        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }

   
        public Site()
        {
             Name = "";
        }

        public new string ToString()
        {
            return Name;
        }
    }



}
