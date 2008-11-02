using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.Common.GDD
{
    class TableSchemeList:List<TableScheme>
    {
        public TableScheme this[string key]
        {
            get
            {
                foreach (TableScheme t in this)
                    if (t.TableName == key || t.NickName == key)
                        return t;

                return null;
            }
        }
    }
}
