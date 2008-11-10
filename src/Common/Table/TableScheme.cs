using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    public class TableSchema:ICloneable
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
        /// 
        /// 这里还有问题，暂时不要使用这个属性
        /// 
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
        /// 返回是主键的属性域
        /// </summary>
        public Field PrimaryKeyField {
            get {
                foreach (Field f in Fields)
                    if (f.IsPrimaryKey)
                        return f;

                return null;
            }
        }

        /// <summary>
        /// 内容字符串
        /// </summary>
        public string Content { get; set; }

        public TableSchema()
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
            {
                if (TableName != "")
                    return TableName;
                else
                    return "*";
            }
            else
            {
                string result = TableName;
                result += "(";
                for (int i = 0; i < Fields.Count; i++)
                {
                    if (i != 0)
                        result += ", ";

                    result += Fields[i].ToString();
                }
                result += ")";
                return result;
            }
        }

        public Field this[string fieldName]
        {
            get
            {
                foreach (Field f in Fields)
                    if (f.AttributeName == fieldName)
                        return f;

                return null;
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            object result = this.MemberwiseClone();
            
            return result;
        }

        #endregion
    }
}
