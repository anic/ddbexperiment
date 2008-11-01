using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Entity
{
    public class TableScheme
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表的别名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 是不是数据库中的表，如果是临时由几个属性组成的“表”，
        /// </summary>
        public bool IsDbTable { get; set; }

        /// <summary>
        /// 表的属性域
        /// </summary>
        public List<Field> Fields { get; set; }

        /// <summary>
        /// 如果是系统表，则是否全属性段
        /// </summary>
        public bool IsAllFields { get; set; }

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public TableScheme()
        {
            TableName = "";
            NickName = "";
            IsDbTable = false;
            Fields = new List<Field>();
            IsAllFields = false;
        }

        public new string ToString()
        {
            if (IsAllFields)
                return TableName + "(*)";
            else
            {
                string result = TableName;
                result += "(";
                for (int i = 0; i < Fields.Count; i++)
                {
                    if (i != 0)
                        result += ", ";

                    result += Fields[i].Content;
                }
                result += ")";
                return result;
            }
        }
    }
}
