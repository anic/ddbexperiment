using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Entity;

namespace DistDBMS.ServerSite.SQLSyntax.Entity
{
    class TableCreation
    {
        TableScheme target;
        public TableScheme Target { get { return target; } }

        public TableCreation()
        {
            target = new TableScheme();
        }

        public new string ToString()
        { 
            //create table Student (id int key, name char(25), sex char(1), age int, degree int)
            string result = "create table" + target.TableName+" (";
            for(int i =0;i<target.Fields.Count;i++)
            {
                if (i != 0)
                    result += ", ";

                if (target.Fields[i].IsPrimaryKey)
                    result += "id " + target.Fields[i].AttributeType.ToString() + " " + target.Fields[i].AttributeName;
                else
                    result += target.Fields[i].AttributeName + " " + target.Fields[i].AttributeType.ToString();
            }
            result +=")";
            return result;
            
        }
    }
}
