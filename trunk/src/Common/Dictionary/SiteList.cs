using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Dictionary
{
    [Serializable]
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
