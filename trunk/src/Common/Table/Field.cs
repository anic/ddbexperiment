using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.Table
{
    /// <summary>
    /// 表中的域
    /// </summary>
    [Serializable]
    public class Field:ICloneable
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 所属的表表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 屏蔽掉分片名称的表名
        /// </summary>
        public string LogicTableName
        {
            get
            {
                int pos = TableName.IndexOf('.');
                if (pos == -1)
                    return TableName;

                return TableName.Substring(0, pos);
            }
        }

        /// <summary>
        /// 屏蔽掉分片名称的属性名
        /// </summary>
        public string LogicAttributeName
        {
            get
            {
                return AttributeName;

                //int pos = TableName.LastIndexOf('.');

                //if (pos == -1)
                //    return TableName;

                //return TableName.Substring(pos + 1, TableName.Length - pos - 1);
            }
        }

        /// <summary>
        /// 属性字段的类型
        /// </summary>
        public AttributeType AttributeType { get; set; }

        /// <summary>
        /// 如Int32后面的32可以存入，可能没用，暂时做着
        /// </summary>
        public object Addition { get; set; }

        /// <summary>
        /// 表中是否以这个属性作为索引
        /// </summary>
        public bool Indexed { get; set; }

        /// <summary>
        /// 表中是否以这个属性作为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        

        public Field()
        {
            AttributeName = "";
            TableName = "";
            Addition = null;
            Indexed = false;
            IsPrimaryKey = false;
            AttributeType = AttributeType.Unknown;
        }

        public new string ToString()
        {
            if (TableName != "")
                return TableName + "." + AttributeName;
            else
                return AttributeName;
        }

        /// <summary>
        /// 比较两个属性是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <param name="isLogic">是否按逻辑表名和属性名进行比较</param>
        /// <returns></returns>
        public bool Equals(Field other, bool isLogic)
        {
            if (isLogic)
                return LogicTableName.Equals(other.LogicTableName) && LogicAttributeName.Equals(other.LogicAttributeName);
            else
                return TableName.Equals(other.TableName) && AttributeName.Equals(other.AttributeName);
        }


        #region ICloneable Members

        public object Clone()
        {
            //return this.MemberwiseClone();
            Field result = new Field();
            result.AttributeName = AttributeName;
            result.TableName = TableName;
            result.Addition = Addition;
            result.Indexed = Indexed;
            result.IsPrimaryKey = IsPrimaryKey;
            result.AttributeType = AttributeType;
            return result;
        }

        #endregion
    }
}
