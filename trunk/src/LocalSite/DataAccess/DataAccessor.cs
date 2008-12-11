using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using System.Data.SQLite;
using DistDBMS.Common;

namespace DistDBMS.LocalSite.DataAccess
{
    class DataAccessor:IDisposable
    {
        
        /// <summary>
        /// 返回最近操作错误（异常）
        /// </summary>
        public Exception LastException { get { return lastEx; } }
        Exception lastEx = null;

        SQLiteConnection conn; //连接，从对象创建到销毁一直连接数据库

        public DataAccessor(string dbName)
        {
            string connection = "Data Source = " + dbName;
            conn = new SQLiteConnection(connection);
            try
            {
                conn.Open();
            }
            catch (Exception ex) { lastEx = ex; }
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetLocalDbType(AttributeType type)
        {
            if (type == AttributeType.String)
                return "TEXT";
            else if (type == AttributeType.Undefined) //如果没有定义，就按照字符串插入
                return "TEXT";
            else
                return type.ToString();
        }

        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public bool CreateTable(TableSchema tableSchema)
        {
            return CreateTable(tableSchema, true, false);
        }

        /// <summary>
        /// 创建表格
        /// </summary>
        /// <param name="tableSchema"></param>
        /// <param name="bPrimaryKey">创建主键</param>
        /// <param name="index">创建索引</param>
        /// <returns></returns>
        public bool CreateTable(TableSchema tableSchema,bool bPrimaryKey, bool index)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "create  table "+ tableSchema.TableName;
            cmd.CommandText += " (";
            for (int i = 0;i<tableSchema.Fields.Count;i++)
            {
                Field f  = tableSchema.Fields[i];
                if (i != 0)
                    cmd.CommandText += ",";

                cmd.CommandText += f.AttributeName + " " + GetLocalDbType(f.AttributeType);

                if (f.IsPrimaryKey && bPrimaryKey)
                    cmd.CommandText += " PRIMARY KEY";
                
            }
            cmd.CommandText += " )";
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                lastEx = ex;
                return false;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int InsertValues(Table table)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            SQLiteTransaction transaction = conn.BeginTransaction();
            try
            {
                for (int i=0;i<table.Tuples.Count;i++)
                {
                    cmd.CommandText = "insert into " + table.Schema.TableName + " values" + table.GenerateTupleString(i);
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
                return table.Tuples.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                transaction.Rollback();
                lastEx = ex;
                return -1;
            }
        }

        /// <summary>
        /// 删除表格
        /// </summary>
        /// <param name="tablename">表格名称</param>
        /// <returns></returns>
        public bool DropTable(string tablename)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "drop table " + tablename;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                lastEx = ex;
                return false;
            }
        }

        /// <summary>
        /// 做单个Select查询
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="schema">目标表样式</param>
        /// <returns>带数据的表格</returns>
        public Table Query(string cmd, TableSchema schema)
        {
            SQLiteCommand sqlCmd = new SQLiteCommand(conn);
            sqlCmd.CommandText = cmd;
            try
            {
                SQLiteDataReader reader = sqlCmd.ExecuteReader();
                if (reader.FieldCount == schema.Fields.Count) //取出的域和要求的样式一致
                {
                    Table result = new Table();
                    result.Schema = schema;
                    int len = schema.Fields.Count;
                    //填充数据
                    while (reader.Read())
                    {
                        Tuple t = new Tuple();
                        for (int i = 0; i < len; i++)
                            t.Data.Add(reader[i].ToString()); //按照string填充
                        result.Tuples.Add(t);
                    }
                    return result;
                }
                else
                    throw new Exception("Data Fields not match schema");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                lastEx = ex;
                return null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();
        }

        #endregion
    }
}
