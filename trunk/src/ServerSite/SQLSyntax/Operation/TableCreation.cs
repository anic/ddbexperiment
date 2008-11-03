using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;

namespace DistDBMS.ServerSite.SQLSyntax.Operation
{
    class TableCreation
    {

        public TableScheme Target { get; set; }

        public TableCreation()
        {
            Target = new TableScheme();
            
        }

        public new string ToString()
        { 
            //create table Student (id int key, name char(25), sex char(1), age int, degree int)
            string result = "create table " + Target.TableName+" (";
            for (int i = 0; i < Target.Fields.Count; i++)
            {
                if (i != 0)
                    result += ", ";

                if (Target.Fields[i].IsPrimaryKey)
                    result += "id " + Target.Fields[i].AttributeType.ToString() + " " + Target.Fields[i].AttributeName;
                else
                    result += Target.Fields[i].AttributeName + " " + Target.Fields[i].AttributeType.ToString();
            }
            result +=")";
            return result;
            
        }
    }
}
