using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    class Selection
    {
        public List<Field> Fields { get; set; } //Select (A.t1,B.t2)

        public List<Table> Sources { get; set; } //From A,B

        public Condition Condition { get; set; } //Where ...
    }
}
