﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using System.Text.RegularExpressions;
using DistDBMS.Common.Dictionary;
using System.IO;

namespace DistDBMS.ControlSite.Plan
{
    class DataImporter
    {
        GlobalDirectory gdd;

        TableSchema currentSchema;

        List<Table> tableList;

        public List<Table> Tables { get { return tableList; } }


        public DataImporter(GlobalDirectory gdd)
        {
            this.gdd = gdd;
            currentSchema = null;
            tableList = new List<Table>();
        }

        public bool ImportFromFile(string filename)
        {
            tableList.Clear();
            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (IsTableDefinition(line)) //是定义
                        {
                            string tablename = GetTableName(line);
                            if (tablename == null)
                                return false;

                            TableSchema schema = GetLogicTableSchema(tablename);
                            if (schema == null)
                                return false;

                            currentSchema = schema; //currentSchema 逻辑表的Schema
                        }
                        else //数据
                        {
                            Tuple tuple = CreateTuple(currentSchema, line, '\t');
                            if (tuple == null)
                                return false;
                            
                            List<Fragment> fragments = GetFragmentByTuple(tuple, currentSchema);
                            foreach (Fragment fragment in fragments)
                            { 
                                Table table = GetTableByFragment(fragment); //将数据插入到对应分片的对应表格中
                                table.Tuples.Add(SplitTuple(tuple, fragment, currentSchema));
                            }
                        }
                    }

                    
                }
            }
            return true;
        }

        /// <summary>
        /// 将一个Tuple转换成满足Fragment的tuple
        /// </summary>
        /// <param name="tuple">原来tuple</param>
        /// <param name="fragment"></param>
        /// <param name="logicSchema">原来Tuple的Schema</param>
        /// <returns></returns>
        private Tuple SplitTuple(Tuple tuple, Fragment fragment,TableSchema logicSchema)
        {
            Tuple result = new Tuple();
            foreach (Field field in fragment.Schema.Fields)
            {
                Field f = logicSchema[field.AttributeName];
                int index = logicSchema.Fields.IndexOf(f);
                result.Data.Add(tuple[index]);
            }
            return result;
        }

        /// <summary>
        /// 根据数据类型，获得对应分片
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private List<Fragment> GetFragmentByTuple(Tuple tuple,TableSchema schema)
        {
            //return gdd.Fragments.GetFragmentByName(schema.TableName);
            List<Fragment> results = new List<Fragment>();
            if (schema.TableName == "Teacher")
            {
                if (Int32.Parse(tuple[0]) < 201000)
                {
                    if (Int32.Parse(tuple[2]) != 3)
                        results.Add(gdd.Fragments.GetFragmentByName("Teacher.1"));
                    else                                                                
                        results.Add(gdd.Fragments.GetFragmentByName("Teacher.2"));

                }
                else
                {
                    if (Int32.Parse(tuple[2]) != 3)
                        results.Add(gdd.Fragments.GetFragmentByName("Teacher.3"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Teacher.4"));
                }
            }
            else if (schema.TableName == "Course")
            {
                results.Add(gdd.Fragments.GetFragmentByName("Course.1"));
                if (Int32.Parse(tuple[3]) == 1)
                    results.Add(gdd.Fragments.GetFragmentByName("Course.2.1"));
                else if (Int32.Parse(tuple[3]) == 4)
                    results.Add(gdd.Fragments.GetFragmentByName("Course.2.3"));
                else
                    results.Add(gdd.Fragments.GetFragmentByName("Course.2.2"));
            }
            else if (schema.TableName == "Exam")
            {
                if (Int32.Parse(tuple[0]) < 107000)
                {
                    if (Int32.Parse(tuple[1]) < 301200)
                        results.Add(gdd.Fragments.GetFragmentByName("Exam.1"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Exam.2"));
                }
                else
                {
                    if (Int32.Parse(tuple[1]) < 301200)
                        results.Add(gdd.Fragments.GetFragmentByName("Exam.3"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Exam.4"));
                }
            }
            else if (schema.TableName == "Student")
            {
                if (Int32.Parse(tuple[0]) < 105000)
                    results.Add(gdd.Fragments.GetFragmentByName("Student.1"));
                else if (Int32.Parse(tuple[0]) >= 110000)
                    results.Add(gdd.Fragments.GetFragmentByName("Student.3"));
                else
                    results.Add(gdd.Fragments.GetFragmentByName("Student.2"));
            }
            else
                results.Add(gdd.Fragments.GetFragmentByName(schema.TableName));
            
            return results;
        }

        private TableSchema GetLogicTableSchema(string tablename)
        {
            Fragment fragment = gdd.Fragments.GetFragmentByName(tablename);
            if (fragment != null)
                return fragment.Schema;
            else
                return null;
        }


        public Table GetTableByFragment(Fragment fragment)
        {
            foreach (Table table in tableList)
                if (table.Name == fragment.Name
                    || table.Schema.NickName == fragment.Name)
                    return table;

            //没有对应的表
            Table result = new Table();
            result.Schema = fragment.Schema.Clone() as TableSchema;
            result.Schema.NickName = fragment.Name;
            result.Schema.TableName = fragment.LogicTable.TableName;
            tableList.Add(result);
            return result;
        
        }

        /// <summary>
        /// 生成一个元组数据
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="rawDatas"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public Tuple CreateTuple(TableSchema schema, string rawDatas,char spliter)
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
    }
}