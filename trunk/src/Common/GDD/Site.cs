using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.GDD
{
    public class Site
    {
        /// <summary>
        /// 站点的IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 站点端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 站点是否活动
        /// </summary>
        public bool IsActive { get; set; }
    }
}
