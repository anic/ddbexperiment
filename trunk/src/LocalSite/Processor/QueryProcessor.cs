using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Execution;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Dictionary;

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

        string condition = "";

        public void SetPackage(ExecutionPackage package)
        { 

        }

        

        public void Handle(ExecutionStep step,string dbname)
        {
            ExecutionRelation root = step.Operation;
            VisitRelation(root);
            target = root.ResultSchema;



            using (DataAccess.DataAccessor accessor = new DistDBMS.ControlSite.DataAccess.DataAccessor(dbname))
            {
                //生成临时表格
                foreach (TableSchema t in tempSources)
                    accessor.CreateTable(t);

                string sql = GenerateQueryString();
                Table result = accessor.Query(sql, target);


                //删除临时表格
                foreach (TableSchema t in tempSources)
                    accessor.DropTable(t.TableName);


            }
        }

        private string GenerateQueryString()
        {
            string result = "select ";
            bool multiSource = (localSources.Count + tempSources.Count > 1);
            for (int i = 0; i < target.Fields.Count; i++)
            {
                if (i != 0)
                    result += ", ";
                if (multiSource)
                    result += target.Fields[i].ToString();
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
            foreach (TableSchema t in tempSources)
            {
                if (index != 0)
                    result += ",";
                result += t.TableName;
                index++;
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
            switch (r.Type)
            {
                case RelationalType.Projection:
                    break;
                case RelationalType.Selection:
                    if (r.IsDirectTableSchema)
                    {
                        if (r.InLocalSite)
                            localSources.Add(r.DirectTableSchema);
                        else
                        {
                            TableSchema outTable = r.DirectTableSchema.Clone() as TableSchema;
                            outTable.TableName = GenerateTempName(outTable.TableName);
                            tempSources.Add(r.DirectTableSchema);
                        }
                    }

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
