using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    public class SiteList : List<Site>
    {
        public Site this[string key]
        {
            get
            {
                foreach (Site s in this)
                    if (s.Name == key)
                        return s;

                return null;
            }
        }

    }
}
