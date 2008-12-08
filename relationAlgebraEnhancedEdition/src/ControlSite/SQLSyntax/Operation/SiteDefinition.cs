using System;
using System.Collections.Generic;
using System.Text;

using DistDBMS.Common.Dictionary;

namespace DistDBMS.ControlSite.SQLSyntax.Operation
{
    class SiteDefinition
    {
         /// <summary>
        /// 构造一个站点对象
        /// </summary>
        public Site Site { get; set; }
        
        

        public SiteDefinition()
        {
            Site = new Site();
            //Content = "";
        }

        public new string ToString()
        { 
            //define site S1 127.0.0.1:2001
            return "define site " + Site.Name + " " + Site.IP + ":" + Site.Port.ToString();
        }

    }
}
