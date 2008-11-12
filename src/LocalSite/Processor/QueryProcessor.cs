 using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Dictionary;
using System.Collections;

namespace DistDBMS.ControlSite.Processor
{

    class QueryProcessor
    {
        TableSchema target = new TableSchema();

        /// <summary>
        /// 从外部来的表格
        /// </summary>
        TableSchemaList tempSources = new TableSchemaList();

        TableSchemaList localSources = new TableSchemaList();

        Hashtable name2Schema = new Hashtable();

        string condition = "";

        
        private Table HandleUnion(ExecutionStep step, string dbname, VirtualBuffer buffer)
        {
            using (DataAccess.DataAccessor accessor = new DistDBMS.ControlSite.DataAccess.DataAccessor(dbname))
            {

                string sql = GenerateQueryString(true);
                System.Console.WriteLine(sql);

                Table result = accessor.Query(sql, target);
                
                //然后将外部的表和这个表在内存中合起来
                System.Console.WriteLine("Union " + localSources[0].TableName + " ," + tempSources[0].TableName);

                return result;
            }
        }

        private Table HandleQuery(ExecutionStep step, string dbname, VirtualBuffer buffer)
        {
            using (DataAccess.DataAccessor accessor = new DistDBMS.ControlSite.DataAccess.DataAccessor(dbname))
            {
                //生成临时表格
                foreach (TableSchema t in tempSources)
                {
                    System.Console.WriteLine("create table " + t.ToString());
                    bool r = accessor.CreateTable(t);
                    if (r)
                    {
                        //填入临时数据
                        
                    }
                }

                
                
                string sql = GenerateQueryString(false);
                System.Console.WriteLine(sql);

                Table result = accessor.Query(sql, target);
                

                //删除临时表格
                foreach (TableSchema t in tempSources)
                {
                    accessor.DropTable(t.TableName);
                    System.Console.WriteLine("drop table " + t.ToString());
                }

                return result;
            }
        }

        public Table Handle(ExecutionStep step,string dbname,VirtualBuffer buffer)
        {
            //重置信息
            tempSources.Clear();
            localSources.Clear();
            condition = "";
            tableIndex = 0;
    
            //遍历关系代数树
            VisitRelation(step.Operation);

            //设置目标
            target = step.Operation.ResultSchema;

            Table result;
            if (step.Operation.Type == RelationalType.Union) //如果是Union ，在内存中做
                result = HandleUnion(step, dbname, buffer);
            else
                result = HandleQuery(step, dbname, buffer); //否则做

            return result;

        }

        private string GetSourceName(string nickname)
        {
            foreach (TableSchema table in localSources)
                if (table.NickName == nickname || table.TableName == nickname)
                    return table.TableName;

            foreach (TableSchema table in tempSources)
                if (table.NickName == nickname || table.TableName == nickname)
                    return table.TableName;

            return nickname;
        }

        /// <summary>
        /// 生成查询语句
        /// </summary>
        /// <param name="bUnion"></param>
        /// <returns></returns>
        private string GenerateQueryString(bool bUnion)
        {
            string result = "select ";
            bool multiSource = (localSources.Count + tempSources.Count > 1);
            for (int i = 0; i < target.Fields.Count; i++)
            {
                if (i != 0)
                    result += ", ";
                if (multiSource)
                    result += GetSourceName(target.Fields[i].TableName) + "." + target.Fields[i].AttributeName;
                else
                    result += target.Fields[i].AttributeName;
            }
            result += " from ";

            int index = 0;
            foreach (TableSchema t in localSources)
            {
                if (index != 0)
                    result += ",";
                result += t.TableName;
                index++;
            }

            if (!bUnion) //如果不是Union，才执行这个操作
            {
                foreach (TableSchema t in tempSources)
                {
                    if (index != 0)
                        result += ",";
                    result += t.TableName;
                    index++;
                }
            }

            if (condition != "")
                result += " where " + condition;

            return result;
        }
        


        static int tableIndex = 0;

        /// <summary>
        /// 生成一个临时的表名
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private string GenerateTempName(string tablename)
        {
            if (tablename!="")
                return tablename + "_temp";
            else
                return "temp"+(tableIndex++).ToString();
        }

        /// <summary>
        /// 浏览每个节点，设置信息
        /// </summary>
        /// <param name="r"></param>
        private void VisitRelation(ExecutionRelation r)
        {
            if (r.IsDirectTableSchema)
            {
                if (r.InLocalSite)
                    localSources.Add(r.DirectTableSchema);
                else
                {
                    if (r.DirectTableSchema.Fields.Count > 0)
                    {
                        TableSchema outTable = r.DirectTableSchema.Clone() as TableSchema;
                        if (outTable.NickName == "") //如果原来有nickname，表名条件中用到
                            outTable.NickName = outTable.TableName;

                        outTable.TableName = GenerateTempName(outTable.TableName);
                        tempSources.Add(outTable);
                    }
                }
            }


            switch (r.Type)
            {
                case RelationalType.Projection:
                    break;
                case RelationalType.Selection:
                    
                    if (r.Predication.Content != "")
                    {
                        if (condition != "")
                            condition += " AND ";
                        condition += r.Predication.Content;
                    }

                    break;
                case RelationalType.Join:
                    {
                        if (condition != "")
                            condition += " AND ";
                        //TODO:找名字
                        //这里只是两个表的join
                        condition += r.RelativeAttributes.Fields[0].ToString() + " = " + r.RelativeAttributes.Fields[1].ToString();
                    }
                    break;
            }

            foreach (ExecutionRelation child in r.Children)
                VisitRelation(child);
        }

        

    }
}
