using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    [Serializable]
    public class Tuple
    {
        /// <summary>
        /// 数据均适用元组形式存储
        /// </summary>
        public List<string> Data { get { return data; } }
        protected List<string> data;

        public Tuple()
        {
            data = new List<string>();
        }

        public Tuple(int size)
        {
            data = new List<string>(size);
        }

        public string this[int index]
        {
            get
            {
                return data[index];
            }
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

        [NonSerialized]
        static StringBuilder sb = new StringBuilder(10 * 1024);
        public string GenerateLineString()
        {
            sb.Length = 0;
            foreach (string str in data)
            {
                sb.Append(str);
                sb.Append("\t");
            }
            return sb.ToString();
        }

        public static Tuple FromLineString(string line)
        {
            string[] items = line.Split('\t');
            Tuple result = new Tuple();
            //添加的时候会多一个\t所以要少一个
            for (int i = 0; i < items.Length - 1; ++i)
                result.data.Add(items[i]);
            return result;
        }
    }
}
