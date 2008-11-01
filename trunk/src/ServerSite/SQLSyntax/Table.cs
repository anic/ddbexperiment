using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    class Table
    {
        public String Name{get;set;}
        
        public String NickName{get;set;}

        List<Field> Fields { get; set; }

        
    }
}
