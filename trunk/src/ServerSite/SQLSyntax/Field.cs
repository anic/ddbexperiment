using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.ServerSite.SQLSyntax
{
    class Field
    {
        public String AttributeName { get; set; }

        public String TableName { get; set; }

        public AttributeType AttributeType { get; set; }

        public Table Table
        {
            get
            {
                return null;
            }
        }

        public Field()
        {
            AttributeName = "UnSet";
            AttributeType = AttributeType.Unkown;
        }
    }
}
