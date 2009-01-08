 using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Dictionary;
using System.Collections;
using DistDBMS.Common.Syntax;

namespace DistDBMS.LocalSite.Processor
{

    class QueryProcessor
    {
        TableSchema target = new TableSchema();

        /// <summary>
        /// 从外部来的表格
        /// </summary>
        TableSchemaList tempSources = new TableSchemaList();
        
        /// <summary>
        /// 表名与Id的映射
        /// </summary>
        Hashtable source2Id = new Hashtable();
        
        /// <summary>
        /// 将旧的表名，替换为新的表名
        /// </summary>
        Hashtable oldname2Newname = new Hashtable();

        /// <summary>
        /// 将混合的表的属性域替换为新的表的属性域
        /// </summary>
        Hashtable oldfield2Newfield = new Hashtable();

        TableSchemaList localSources = new TableSchemaList();

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
            using (DataAccess.DataAccessor accessor = new DistDBMS.LocalSite.DataAccess.DataAccessor(dbname))
            {

                string sql = GenerateQueryString(true);
                System.Diagnostics.Debug.WriteLine(sql);

                target.ReplaceTableName(localSources[0].TableName);
                Table result = accessor.Query(sql, target);
                if (result == null)
                    throw new Exception("sql:" + sql + " error.Info" + accessor.LastException.Message);

                string union = "Union " + localSources[0].TableName;
                foreach (TableSchema schema in tempSources)
                    union += " ," + schema.TableName;
                System.Diagnostics.Debug.WriteLine(union);

                //然后将外部的表和这个表在内存中合起来

                foreach (TableSchema t in tempSources)
                {
                    //填入临时数据
                    Table tmpTable = buffer.GetPackageById((int)source2Id[t.TableName]).Object as Table;
                    result.Tuples.AddRange(tmpTable.Tuples);
                }


                return result;
            }
        }

        private Table HandleQuery()
        {
            using (DataAccess.DataAccessor accessor = new DistDBMS.LocalSite.DataAccess.DataAccessor(dbname))
            {

                //生成临时表格
                foreach (TableSchema t in tempSources)
                {
                    System.Diagnostics.Debug.WriteLine("create table " + t.ToString());
                    bool r = accessor.CreateTable(t, false, false);
                    if (r)
                    {
                        //填入临时数据
                        Table tmpTable = buffer.GetPackageById((int)source2Id[t.TableName]).Object as Table;
                        tmpTable.Schema = t;
                        int nResult = accessor.InsertValues(tmpTable);
                    }
                }

                
                
                string sql = GenerateQueryString(false);
                System.Diagnostics.Debug.WriteLine(sql);
                Table result = accessor.Query(sql, target);
                if (result == null)
                    throw new Exception("sql:" + sql + " execute error. Info: " + accessor.LastException.Message);
       
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
            tmpIndex = 0;
            source2Id.Clear();
            oldname2Newname.Clear();
            oldfield2Newfield.Clear();
            this.step = step;
            this.dbname = dbname;
            this.buffer = buffer;

            //遍历关系代数树
            VisitRelation(step.Operation);

            //设置目标
            target = step.Operation.ResultSchema.Clone() as TableSchema;
            

            Table result;
            if (step.Operation.Type == RelationalType.Union) //如果是Union ，在内存中做
                result = HandleUnion();
            else
            {
                ModifyTarget();
                result = HandleQuery(); //否则做Select
            }

            return result;

        }

        private TableSchema GetTableSchema(string name)
        {
            TableSchema result = localSources[name];
            if (result != null)
                return result;
            else
                return tempSources[name];
        }

        private void ReplaceField(Field field)
        {
            string tablename = GetLocalTablename(field.TableName);
            string attributename = field.AttributeName;
                //如果有映射的表名，则修改之，并检查属性是否存在于该表
                //还需要检查该属性是否已经更名
                if (tablename != null)
                {
                    TableSchema table = GetTableSchema(tablename);
                    //if (table[attributename] != null)   //有该属性
                    //    field.TableName = tablename;
                    
                    //无论是否有属性，也得检查一下是否已经改名
                    //else
                    {
                        //如果没有该属性，则查看属性变更表
                        string fieldname = (string)this.oldfield2Newfield[field.TableName + "." + field.AttributeName];
                        if (fieldname != null)
                            field.AttributeName = fieldname;
                    }
                    field.TableName = tablename;
                }
                else//如果没有映射表名，则找到有相同属性的表，如果找到只有一个，则填写该表名
                {
                    int found = 0;
                    bool localfound = false;
                    foreach (TableSchema schema in localSources)
                        if (schema[attributename] != null)
                        {
                            tablename = schema.TableName;
                            found++;
                            localfound = true;
                        }

                    foreach (TableSchema schema in tempSources)
                        if (schema[attributename] != null)
                        {
                            tablename = schema.TableName;
                            found++;
                        }

                    if (found == 1)
                    {
                        field.TableName = tablename;
                    }
                    else      //如果没有映射表名，多个表都有这个属性
                    {
                        if (localfound && localSources.Count == 1)  //如果本地的表有，且只有一个本地表
                            field.TableName = localSources[0].TableName;
                        else
                        {
                            //throw new Exception();
                            //Common.Debug.Assert(false, "ReplaceField 没有找到对应的表");
                        }
                    }          
                    
                }
        }

        private void ModifyTarget()
        {
            for (int i = 0; i < target.Fields.Count; i++)
                ReplaceField(target.Fields[i]);
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
       
            //TODO:添加对* 的判断
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
                        if (bUnion)
                            result += target.Fields[i].AttributeName;
                        else
                        {
                            /*string name1 = GetLocalTablename(target.Fields[i].TableName);
                            result += name1 + "." + target.Fields[i].AttributeName;*/
                            result += target.Fields[i].TableName + "." + target.Fields[i].AttributeName;
                        }
                        
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
                    condition.AtomCondition.LeftOperand.Field = condition.AtomCondition.LeftOperand.Field.Clone() as Field;

                    //string name = this.GetLocalTablename(condition.AtomCondition.LeftOperand.Field.TableName);
                    //if (name != null)
                    //    condition.AtomCondition.LeftOperand.Field.TableName = name;
                    //else
                        ReplaceField(condition.AtomCondition.LeftOperand.Field);//如果给的是逻辑表名，如Course，因为local中也是用该作为表名，所以不用变化
                }

                if (condition.AtomCondition.RightOperand.IsField)
                {
                    condition.AtomCondition.RightOperand.Field = condition.AtomCondition.RightOperand.Field.Clone() as Field;

                    //string name = this.GetLocalTablename(condition.AtomCondition.RightOperand.Field.TableName);
                    //if (name != null)
                    //    condition.AtomCondition.RightOperand.Field.TableName = name;
                    //else
                        ReplaceField(condition.AtomCondition.RightOperand.Field);//如果给的是逻辑表名，如Course，因为local中也是用该作为表名，所以不用变化
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


        static int tmpIndex = 0;

        /// <summary>
        /// 生成一个临时的表名
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private string GenerateTempName(string tablename)
        {
            if (tablename != "")
            {
                int index = tablename.IndexOf('.'); //去除所有.
                if (index != -1)
                    tablename = tablename.Substring(0, index);

                return tablename + "_temp" + (tmpIndex++).ToString();
            }
            else
                return "temp" + (tmpIndex++).ToString();
        }

        private string GenerateTempFieldName(string fieldname)
        {
            return fieldname + (tmpIndex++).ToString();
        }

        private void SetNewname(TableSchema schema,string newname)
        {
            //TODO:Course_teacher(Course.id,Teacher.id,....) 如果有条件Course.id = Teacher.id，则如果Course_teacher更改表名，则这个信息就会被掩盖了

            if (schema.IsMixed)
            {
                for (int i = 0; i < schema.Fields.Count; i++)
                {
                    oldname2Newname[schema.Fields[i].TableName] = newname;
                    for(int j = i+1;j<schema.Fields.Count;j++)
                        if (schema.Fields[j].AttributeName == schema.Fields[i].AttributeName) //有相同的名字
                        {
                            string newField =GenerateTempFieldName(schema.Fields[j].AttributeName);
                            oldfield2Newfield[schema.Fields[j].TableName + "." + schema.Fields[j].AttributeName] = newField;
                            //更换了属性名
                            schema.Fields[j].AttributeName = newField;
                        }
                }
            }
            oldname2Newname[schema.TableName] = newname;    
            schema.ReplaceTableName(newname);
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
                    Fragment fragment = ldd.Fragments.GetFragmentByName(r.DirectTableSchema.TableName);
                    TableSchema result = r.DirectTableSchema.Clone() as TableSchema;
                    //设置改变的名称
                    SetNewname(result, fragment.LogicSchema.TableName);

                    localSources.Add(result);
                }
                else
                {
                    if (r.DirectTableSchema.Fields.Count > 0)
                    {
                        TableSchema outTable = r.DirectTableSchema.Clone() as TableSchema;
                        
                        string tempName = GenerateTempName(outTable.TableName);
                        //设置改变的名称
                        SetNewname(outTable, tempName);
                        
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


        private string GetLocalTablename(string tablename)
        {
            return (string)oldname2Newname[tablename];
        }

     
        

    }
}
