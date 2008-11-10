using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace DistDBMS.Common.Table
{
    /// <summary>
    /// 一个包含数据的表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 表头，记录了表名、属性名、属性的类型（int）
        /// </summary>
        public TableSchema Schema { get; set; }

        /// <summary>
        /// 元组列表
        /// </summary>
        public List<Tuple> Tuples { get { return tuples; } }
        List<Tuple> tuples;

        /// <summary>
        /// 表格名称
        /// </summary>
        public string Name {
            get {
                if (Schema != null)
                    return Schema.TableName;
                else
                    return "";
            }
        }

        public Table() {
            Schema = new TableSchema();
            tuples = new List<Tuple>();
        }

        /// <summary>
        /// 返回Tuple的字符串，会根据类型进行处理，如(1,'aaa')
        /// </summary>
        /// <param name="index">tuple的索引</param>
        /// <returns></returns>
        public string GenerateTupleString(int index)
        {
            if (index >= 0 && index < tuples.Count)
            {
                if (Schema != null && tuples[index].Data.Count == Schema.Fields.Count)
                {
                    string result ="(";
                    Tuple t = tuples[index];
                    for (int i = 0; i < t.Data.Count; i++)
                    {
                        if (i!=0)
                            result +=", ";

                        if (Schema.Fields[i].AttributeType == AttributeType.String)
                            result += "\'" + t.Data[i] + "\'";
                        else
                            result += t.Data[i];
                    }
                    result += ")";
                    return result;
                }
                else
                    return tuples[index].ToString();
            }
            else
                throw new IndexOutOfRangeException();
        }
    }
}
