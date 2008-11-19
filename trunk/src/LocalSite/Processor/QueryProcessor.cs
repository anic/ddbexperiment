 using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Dictionary;
using System.Collections;
using DistDBMS.Common.Syntax;

namespace DistDBMS.ControlSite.Processor
{

    class QueryProcessor
    {
        TableSchema target = new TableSchema();

        /// <summary>
        /// 从外部来的表格
        /// </summary>
        TableSchemaList tempSources = new TableSchemaList();
        Hashtable source2Id = new Hashtable();

        TableSchemaList localSources = new TableSchemaList();

        Hashtable name2Schema = new Hashtable();

        List<Condition> conditions = new List<Condition>();


        LocalDirectory ldd;

        ExecutionStep step;
        string dbname;
        VirtualBuffer buffer;

        public QueryProcessor(LocalDirectory ldd)
        {
            this.ldd = ldd;
        }

        
        private Table HandleUnion()
        {
            using (DataAccess.DataAccessor accessor = new DistDBMS.ControlSite.DataAccess.DataAccessor(dbname))
            {

                string sql = GenerateQueryString(true);
                System.Diagnostics.Debug.WriteLine(sql);

                Table result = accessor.Query(sql, target);
                
       
                string union = "Union " + localSources[0].TableName;
                foreach (TableSchema schema in tempSources)
                    union += " ," + schema.TableName;
                System.Diagnostics.Debug.WriteLine(union);

                //然后将外部的表和这个表在内存中合起来

                foreach (TableSchema t in tempSources)
                {
                    //填入临时数据
                    Table tmpTable = buffer[(string)source2Id[t.TableName]].Object as Table;
                    result.Tuples.AddRange(tmpTable.Tuples);
                }


                return result;
            }
        }

        private Table HandleQuery()
        {
            using (DataAccess.DataAccessor accessor = new DistDBMS.ControlSite.DataAccess.DataAccessor(dbname))
            {

                //生成临时表格
                foreach (TableSchema t in tempSources)
                {
                    System.Diagnostics.Debug.WriteLine("create table " + t.ToString());
                    bool r = accessor.CreateTable(t, false, false);
                    if (r)
                    {
                        //填入临时数据
                        Table tmpTable = buffer[(string)source2Id[t.TableName]].Object as Table;
                        tmpTable.Schema = t;
                        int nResult = accessor.InsertValues(tmpTable);
                    }
                }

                
                
                string sql = GenerateQueryString(false);
                System.Diagnostics.Debug.WriteLine(sql);

                Table result = accessor.Query(sql, target);
       
                //删除临时表格
                foreach (TableSchema t in tempSources)
                {
                    accessor.DropTable(t.TableName);
                    System.Diagnostics.Debug.WriteLine("drop table " + t.ToString());
                }

                return result;
            }
        }

        public Table Handle(ExecutionStep step,string dbname,VirtualBuffer buffer)
        {
            //重置信息
            tempSources.Clear();
            localSources.Clear();
            conditions.Clear();
            tableIndex = 0;
            source2Id.Clear();

            this.step = step;
            this.dbname = dbname;
            this.buffer = buffer;

            //遍历关系代数树
            VisitRelation(step.Operation);

            //设置目标
            target = step.Operation.ResultSchema;

            Table result;
            if (step.Operation.Type == RelationalType.Union) //如果是Union ，在内存中做
                result = HandleUnion();
            else
                result = HandleQuery(); //否则做Select

            return result;

        }

        /// <summary>
        /// 通过nickname获得对应的表名
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private string GetSourceName(string nickname)
        {
            foreach (TableSchema table in localSources)
                if (table.NickName == nickname || table.TableName == nickname)
                    return table.TableName;

            foreach (TableSchema table in tempSources)
                if (table.NickName == nickname || table.TableName == nickname)
                    return table.TableName;

            //近似匹配
            foreach (TableSchema table in localSources)
                if (table.NickName.IndexOf(nickname) != -1 || table.TableName.IndexOf(nickname) != -1)
                    return table.TableName;

            foreach (TableSchema table in tempSources)
                if (table.NickName.IndexOf(nickname) != -1 || table.TableName.IndexOf(nickname) != -1)
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
            bool isAllField = false;
            /*
            if (!multiSource && localSources.Count == 1 && target.Fields.Count == localSources[0].Fields.Count)
            {
                isAllField = true;
                for (int i = 0; i < target.Fields.Count; i++)
                    isAllField &= (target.Fields[i].TableName == localSources[0].Fields[i].TableName
                        && target.Fields[i].AttributeName == localSources[0].Fields[i].AttributeName);
            }*/
            

            if (isAllField)
                result += "*";
            else
            {
                for (int i = 0; i < target.Fields.Count; i++)
                {
                    if (i != 0)
                        result += ", ";
                    if (multiSource)
                    {
                        string name1 = FindTablenameByField(target.Fields[i].AttributeName, target.Fields[i].TableName);
                        result += name1 + "." + target.Fields[i].AttributeName;
                        
                    }
                    else
                        result += target.Fields[i].AttributeName;
                }
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

                if (conditions.Count > 0)
                {
                    //替换条件中的表名
                    ReplaceAllConditionField();

                    result += " where ";
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        if (i != 0)
                            result += " AND " + conditions[i].ToString();
                        else
                            result += conditions[i].ToString();
                    }
                }

            }

            

            return result;
        }

        private void ReplaceAllConditionField()
        {
            //foreach (TableSchema schema in localSources)
            //{
            //    foreach (Condition condition in conditions)
            //        ReplaceConditionField(condition, schema.NickName, schema.TableName);
            //}

            //foreach (TableSchema schema in tempSources)
            //{
            //    foreach (Condition condition in conditions)
            //        ReplaceConditionField(condition, schema.NickName, schema.TableName);
            //}

            foreach (Condition condition in conditions)
            {
                ReplaceConditionField(condition);
            }
        }

        private void ReplaceConditionField(Condition condition)
        { 
            if (condition.IsAtomCondition)
            {
                if (condition.AtomCondition.LeftOperand.IsField)
                {
                    string name = this.FindTablenameByField(condition.AtomCondition.LeftOperand.Field.AttributeName, 
                        condition.AtomCondition.LeftOperand.Field.TableName);

                    if (name != "")
                    {
                        condition.AtomCondition.LeftOperand.Field = condition.AtomCondition.LeftOperand.Field.Clone() as Field;
                        condition.AtomCondition.LeftOperand.Field.TableName = name;
                    }
                }

                if (condition.AtomCondition.RightOperand.IsField)
                {
                    string name = this.FindTablenameByField(condition.AtomCondition.RightOperand.Field.AttributeName,
                        condition.AtomCondition.RightOperand.Field.TableName);

                    if (name != "")
                    {
                        condition.AtomCondition.RightOperand.Field = condition.AtomCondition.RightOperand.Field.Clone() as Field;
                        condition.AtomCondition.RightOperand.Field.TableName = name;
                    }
                }
            }
            else
            {
                if (condition.LeftCondition != null)
                    ReplaceConditionField(condition.LeftCondition);

                if (condition.RightCondition != null)
                    ReplaceConditionField(condition.RightCondition);
            }

            
        }

        private void ReplaceConditionField(Condition condition,string oldTablename,string newTablename)
        {
            if (condition.IsAtomCondition)
            {
                if (condition.AtomCondition.LeftOperand.IsField
                    && condition.AtomCondition.LeftOperand.Field.TableName == oldTablename)
                    condition.AtomCondition.LeftOperand.Field.TableName = newTablename;

                if (condition.AtomCondition.RightOperand.IsField
                    && condition.AtomCondition.RightOperand.Field.TableName == oldTablename)
                    condition.AtomCondition.RightOperand.Field.TableName = newTablename;
            }
            else
            {
                if (condition.LeftCondition != null)
                    ReplaceConditionField(condition.LeftCondition, oldTablename, newTablename);

                if (condition.RightCondition != null)
                    ReplaceConditionField(condition.RightCondition, oldTablename, newTablename);
            }
        }
        


        static int tableIndex = 0;

        /// <summary>
        /// 生成一个临时的表名
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private string GenerateTempName(string tablename)
        {
            if (tablename != "")
                return tablename + "_temp" + (tableIndex++).ToString();
            else
                return "temp" + (tableIndex++).ToString();
        }

        

        /// <summary>
        /// 浏览每个节点，设置信息
        /// </summary>
        /// <param name="r"></param>
        private void VisitRelation(ExecutionRelation r)
        {
            if (r.IsDirectTableSchema)
            {
                if (r.InLocalSite && !step.IsWaiting(r.ResultID))
                {
                    localSources.Add(r.DirectTableSchema);
                }
                else
                {
                    if (r.DirectTableSchema.Fields.Count > 0)
                    {
                        TableSchema outTable = r.DirectTableSchema.Clone() as TableSchema;
                        if (outTable.NickName == "") //如果原来有nickname，表名条件中用到
                            outTable.NickName = outTable.TableName;

                        outTable.TableName = GenerateTempName(outTable.TableName);

                        //将域中的表名替换为临时表的 表名
                        for (int i = 0; i < outTable.Fields.Count; i++)
                        {
                            outTable.Fields[i] = outTable.Fields[i].Clone() as Field;
                            outTable.Fields[i].TableName = outTable.TableName;
                        }

                        tempSources.Add(outTable);
                        source2Id[outTable.TableName] = r.ResultID;
                    }
                }
            }


            switch (r.Type)
            {
                case RelationalType.Projection:
                    break;
                case RelationalType.Selection:
                    {
                        if (!r.Predication.IsEmpty)
                            conditions.Add(r.Predication);
                        break;
                    }
                case RelationalType.Join:
                    {
                        //TODO:找名字
                        //这里只是两个表的join
                        Condition newCondition = new Condition();
                        newCondition.AtomCondition = new AtomCondition();
                        newCondition.AtomCondition.LeftOperand.Field = r.RelativeAttributes.Fields[0].Clone() as Field;
                        newCondition.AtomCondition.LeftOperand.IsField = true;
                        newCondition.AtomCondition.RightOperand.Field = r.RelativeAttributes.Fields[1].Clone() as Field;
                        newCondition.AtomCondition.RightOperand.IsField = true;
                        conditions.Add(newCondition);
                    }
                    break;
            }

            foreach (ExecutionRelation child in r.Children)
                VisitRelation(child);
        }

        private string FindTablenameByField(string fieldname, string tablename)
        {
            int found = 0;
            string result = "";
            foreach (TableSchema schema in localSources)
                foreach (Field f in schema.Fields)
                    if (f.AttributeName == fieldname)
                    {
                        if (result == "")
                            result = schema.TableName;

                        found++;
                    }

            foreach (TableSchema schema in tempSources)
                foreach (Field f in schema.Fields)
                    if (f.AttributeName == fieldname)
                    {
                        if (result == "")
                            result = schema.TableName;

                        found++;
                    }



            if (found == 1)
                return result;
            else//  found ==0 || found >1
                return GetSourceName(tablename);

        }
        

    }
}
