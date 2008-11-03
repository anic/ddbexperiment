using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    public class Tuple
    {
        /// <summary>
        /// 数据均适用元组形式存储
        /// </summary>
        public List<string> Data { get { return data; } }
        List<string> data;

        public Tuple()
        {
            data = new List<string>();
        }

        public new string ToString()
        {
            string result = "(";
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                    result += ", ";
                result += data[i].ToString();
            }
            result += ")";

            return result;
        }
    }
}
