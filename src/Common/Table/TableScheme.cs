using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Syntax;

namespace DistDBMS.Common
{
    public class TableSchema : ICloneable
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
        /// 标签，优化时使用
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// 与Table相关的一元谓词，优化时使用
        /// </summary>
        public List<AtomCondition> RelativeUnaryPredication { get; set; }

        /// <summary>
        /// 返回是主键的属性域
        /// </summary>
        public Field PrimaryKeyField
        {
            get
            {
                foreach (Field f in Fields)
                    if (f.IsPrimaryKey)
                        return f;

                return null;
            }
        }

        public bool IsMixed
        {
            get
            {
                //检查是否只有一个表
                if (TableName == "")
                    return true;

                bool sameTable = true;
                foreach (Field f in Fields)
                    sameTable &= (f.TableName == TableName);
                return !sameTable;
            }
        }

        /// <summary>
        /// 将这个Schema里所有表名替换
        /// </summary>
        /// <param name="tablename"></param>
        public void ReplaceTableName(string tablename)
        {
            this.TableName = tablename;
            foreach (Field field in Fields)
                field.TableName = tablename;
        }

        public TableSchema()
        {
            TableName = "";
            NickName = "";
            IsDbTable = false;
            Fields = new List<Field>();
            IsAllFields = false;
            Tag = -1;
            RelativeUnaryPredication = new List<AtomCondition>();
        }

        public new string ToString()
        {
            //检查是否只有一个表
            bool sameTable = !IsMixed;

            string result = TableName;
            result += "(";
            for (int i = 0; i < Fields.Count; i++)
            {
                if (i != 0)
                    result += ", ";

                if (!sameTable) //所有Field的Tablename和Tablemane一致，则不写
                    result += Fields[i].TableName + ".";

                result += Fields[i].AttributeName;
            }
            result += ")";
            return result;

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
            TableSchema result = new TableSchema();
            result.TableName = TableName;
            result.NickName = NickName;
            result.IsDbTable = IsDbTable;
            result.Tag = Tag;
            foreach (Field f in Fields)
                result.Fields.Add(f.Clone() as Field);
            result.IsAllFields = IsAllFields;

            foreach (AtomCondition atom in RelativeUnaryPredication)
                result.RelativeUnaryPredication.Add(atom.Clone() as AtomCondition);

            return result;
        }

        #endregion
    }
}
