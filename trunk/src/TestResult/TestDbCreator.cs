using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistDBMS.ControlSite;
using System.IO;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using System.Text.RegularExpressions;
using DistDBMS.LocalSite.DataAccess;
using DistDBMS.TestResult.Properties;

namespace DistDBMS.TestResult
{
    class TestDbCreator
    {
        GlobalDirectory gdd;
        public static string FILE_DB_TESTER = "TESTER";
        List<Table> tableList = new List<Table>();

        public TestDbCreator()
        {

        }

        public void ImportData(string filename)
        {
            using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
            {
                while (!sr.EndOfStream)
                    HandleLine(sr.ReadLine());

                sr.Close();
            }
            foreach (Table t in tableList)
            {
                using (DataAccessor da = new DataAccessor(FILE_DB_TESTER))
                {
                    da.CreateTable(t.Schema);
                    da.InsertValues(t);
                }
            }
        }

        private Table GetTable(string name)
        {
            foreach (Table table in tableList)
                if (table.Name == name)
                    return table;

            //没有对应的表
            Table result = new Table();
            result.Schema = gdd.Schemas[name].Clone() as TableSchema;
            tableList.Add(result);
            return result;

        }

        /// <summary>
        /// 是不是定义表
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private bool IsTableDefinition(string raw)
        {
            return (raw.IndexOf('(') != -1);
        }

        /// <summary>
        /// 获得表名字
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        private string GetTableName(string raw)
        {
            if (IsTableDefinition(raw))
            {
                int index = raw.IndexOf('(');
                return raw.Substring(0, index).Trim();
            }
            return null;
        }

        /// <summary>
        /// 生成一个元组数据
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="rawDatas"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public Tuple CreateTuple(TableSchema schema, string rawDatas, char spliter)
        {
            string[] datas = rawDatas.Split(spliter);
            if (datas.Length == schema.Fields.Count)
            {
                Tuple result = new Tuple();
                for (int i = 0; i < datas.Length; i++)
                {
                    result.Data.Add(ExtractData(datas[i], schema.Fields[i]));
                }

                return result;
            }
            else
                return null;
        }

        /// <summary>
        /// 将字符类型数据去除引号
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private string ExtractData(string rawData, Field field)
        {
            if (field.AttributeType == DistDBMS.Common.AttributeType.String)
            {
                Regex reg = new Regex("\'(.*)\'");
                Match match = reg.Match(rawData);
                if (match.Success)
                    return match.Groups[1].ToString();

                reg = new Regex("\"(.*)\"");
                match = reg.Match(rawData);
                if (match.Success)
                    return match.Groups[1].ToString();

            }

            return rawData;
        }



        TableSchema currentSchema;

        private bool HandleLine(string line)
        {
            if (IsTableDefinition(line)) //是定义
            {
                string tablename = GetTableName(line);
                if (tablename == null)
                    return false;

                TableSchema schema = gdd.Schemas[tablename];
                if (schema == null)
                    return false;

                currentSchema = schema; //currentSchema 逻辑表的Schema
            }
            else //数据
            {
                Tuple tuple = CreateTuple(currentSchema, line, ',');
                if (tuple == null)
                    return false;

                Table table = GetTable(currentSchema.TableName); //将数据插入到对应分片的对应表格中
                table.Tuples.Add(tuple);
            }

            return true;
        }

        public void LoadTestGDD(string filename)
        {

            GDDCreator creator = new GDDCreator();
            creator.InitCreatioin();

            string[] lines = Resources.DbInitScriptTest.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
                creator.InsertCommand(line);


            gdd = creator.CreateGDD();
        }
    }
}
