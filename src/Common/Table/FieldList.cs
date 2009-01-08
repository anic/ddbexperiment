using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    public class FieldList:List<Field>
    {
        public Field this[Field key]
        {
            get
            {
                foreach (Field f in this)
                {
                    if (key.AttributeName.Equals(f.AttributeName) && key.TableName.Equals(f.TableName))
                        return f;
                }
                return null;

            }            
        }

        public Field FindLogic(Field key)
        {
            foreach (Field f in this)
            {
                if (key.LogicAttributeName.Equals(f.LogicAttributeName) && key.LogicTableName.Equals(f.LogicTableName))
                    return f;
            }
            return null;
        }
    }
}
