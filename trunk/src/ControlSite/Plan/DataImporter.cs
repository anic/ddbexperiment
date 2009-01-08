using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.Table;
using System.Text.RegularExpressions;
using DistDBMS.Common.Dictionary;
using System.IO;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.Common.Syntax;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.ControlSite.RelationalAlgebraUtility;

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

        public bool ImportFromText(string[] lines)
        {
            tableList.Clear();
            foreach (string line in lines)
                HandleLine(line);

            return true;
        }

        public bool ImportFromSql(Insertion ins)
        {
            tableList.Clear();
            List<Fragment> fragments = GetFragmentByTuple(ins.Data, ins.Target);
            foreach (Fragment fragment in fragments)
            {
                Table table = GetTableByFragment(fragment); //将数据插入到对应分片的对应表格中
                table.Tuples.Add(SplitTuple(ins.Data, fragment, ins.Target));
            }
            return true;
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
                        HandleLine(line);
                    }
                }
            }
            return true;
        }

        private bool HandleLine(string line)
        {
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
                Tuple tuple = CreateTuple(currentSchema, line, ',');
                if (tuple == null)
                    return false;

                List<Fragment> fragments = GetFragmentByTuple(tuple, currentSchema);
                foreach (Fragment fragment in fragments)
                {
                    Table table = GetTableByFragment(fragment); //将数据插入到对应分片的对应表格中
                    table.Tuples.Add(SplitTuple(tuple, fragment, currentSchema));
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
        /// 获取分片的所有约束条件谓词
        /// </summary>
        /// <param name="fragment"></param>
        /// <returns></returns>
        private List<AtomCondition> GetFragmentConditions(Fragment fragment)
        {
            List<AtomCondition> results = new List<AtomCondition>();

            if (fragment == null || fragment.Condition == null || fragment.Condition.IsEmpty)
                return results;

            ConditionConverter conditionConverter = new ConditionConverter();
            conditionConverter.Convert(fragment.Condition, NormalFormType.Conjunction);
            ConjunctiveNormalForm normalForm = conditionConverter.ConjunctionNormalForm;
            results.AddRange(normalForm.PredicationItems);

            results.AddRange(GetFragmentConditions(fragment.Parent));

            return results;
        }

        /// <summary>
        /// 在GDD中查找所有与tuple不冲突的Fragment
        /// </summary>
        /// <param name="fragment"></param>
        /// <param name="tupleAttributeConditions"></param>
        /// <param name="result"></param>
        private void GetUnconflictFragments(Fragment fragment, List<AtomCondition> tupleAttributeConditions, ref List<Fragment> result)
        {
            // 叶子结点
            if (fragment.Children.Count == 0)
            {
                List<AtomCondition> fragmentConditions = GetFragmentConditions(fragment);
                foreach (AtomCondition fragmentAtom in fragmentConditions)
                {
                    foreach (AtomCondition tupleAtom in tupleAttributeConditions)
                    {
                        if (fragmentAtom.ConflictWith(tupleAtom))
                            return;
                    }
                }

                result.Add(fragment);
            }
            else // 中间分片结点
            {
                foreach (Fragment f in fragment.Children)
                    GetUnconflictFragments(f, tupleAttributeConditions, ref result);
            }
            
        }

        /// <summary>
        /// 根据数据类型，获得对应分片
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        private List<Fragment> GetFragmentByTuple(Tuple tuple,TableSchema schema)
        {

            //TODO:需要进行动态测试
            List<Fragment> results = new List<Fragment>();

            List<AtomCondition> tupleAttributeCondition = new List<AtomCondition>();

            int pos;
            for (pos = 0; pos < schema.Fields.Count; pos++)
            {
                AtomCondition atom = new AtomCondition();
                atom.Operator = DistDBMS.Common.LogicOperator.Equal;
                switch (schema.Fields[pos].AttributeType)
                {
                    case DistDBMS.Common.AttributeType.Int:
                        atom.LeftOperand.Field = schema.Fields[pos].Clone() as Field;
                        atom.LeftOperand.IsField = true;
                        atom.RightOperand.ValueType = DistDBMS.Common.AttributeType.Int;
                        atom.RightOperand.Value = Convert.ToInt32(tuple.Data[pos]);
                        break;
                    case DistDBMS.Common.AttributeType.String:
                        atom.LeftOperand.Field = schema.Fields[pos].Clone() as Field;
                        atom.LeftOperand.IsField = true;
                        atom.RightOperand.ValueType = DistDBMS.Common.AttributeType.String;
                        atom.RightOperand.Value = tuple.Data[pos];
                        break;
                }

                tupleAttributeCondition.Add(atom);
            }

            ////////////////////////////////////////////////////////////////////
            //
            // 检查每一个tuple中的键值对是否与一个分片的所有约束存在冲突
            // 若存在，本tuple不属于该分片，否则将分片加入results中
            // 

            List<Fragment> candidateFragments = new List<Fragment>();

            // 先检查分片Condition冲突
            foreach (Fragment f in gdd.Fragments)
            {
                List<Fragment> accept = new List<Fragment>();
                GetUnconflictFragments(f, tupleAttributeCondition, ref accept);
                candidateFragments.AddRange(accept);
            }

            
            // 检查属性冲突，如果分片中没有属性属于schema，则该分片不是所需分片
            bool acceptFragment;
            foreach (Fragment frag in candidateFragments)
            {
                acceptFragment = false;
                foreach (Field field in frag.Schema.Fields)
                {
                    foreach (Field tupleAttr in schema.Fields)
                    {
                        if (field.Equals(tupleAttr, true))
                        {
                            results.Add(frag);
                            acceptFragment = true;
                            break;
                        }
                    }
                    if (acceptFragment)
                        break;
                }
            }
            
            /*
            
            if (schema.TableName == "Producer")
            {
                if (Int32.Parse(tuple[0]) < 200200)
                {
                    if (tuple[2] == "BJ")
                        results.Add(gdd.Fragments.GetFragmentByName("Producer.1"));
                    else                                                                
                        results.Add(gdd.Fragments.GetFragmentByName("Producer.2"));

                }
                else
                {
                    if (tuple[2] == "BJ")
                        results.Add(gdd.Fragments.GetFragmentByName("Producer.3"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Producer.4"));
                }
            }
            else if (schema.TableName == "Product")
            {
                results.Add(gdd.Fragments.GetFragmentByName("Product.1"));
                if (Int32.Parse(tuple[3]) < 2000)
                    results.Add(gdd.Fragments.GetFragmentByName("Product.2.1"));
                else if (Int32.Parse(tuple[3]) >= 4000)
                    results.Add(gdd.Fragments.GetFragmentByName("Product.2.3"));
                else
                    results.Add(gdd.Fragments.GetFragmentByName("Product.2.2"));
            }
            else if (schema.TableName == "Purchase")
            {
                if (Int32.Parse(tuple[0]) < 107000)
                {
                    if (Int32.Parse(tuple[1]) < 310000)
                        results.Add(gdd.Fragments.GetFragmentByName("Purchase.1"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Purchase.2"));
                }
                else
                {
                    if (Int32.Parse(tuple[1]) < 310000)
                        results.Add(gdd.Fragments.GetFragmentByName("Purchase.3"));
                    else
                        results.Add(gdd.Fragments.GetFragmentByName("Purchase.4"));
                }
            }
            else if (schema.TableName == "Customer")
            {
                if (Int32.Parse(tuple[0]) < 110000)
                    results.Add(gdd.Fragments.GetFragmentByName("Customer.1"));
                else if (Int32.Parse(tuple[0]) >= 110000 && Int32.Parse(tuple[0]) < 112500)
                    results.Add(gdd.Fragments.GetFragmentByName("Customer.2"));
                else
                    results.Add(gdd.Fragments.GetFragmentByName("Customer.3"));
            }
            else
                results.Add(gdd.Fragments.GetFragmentByName(schema.TableName));
            */
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
            //result.Schema.NickName = fragment.Name;
            //result.Schema.TableName = fragment.LogicTable.TableName;
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
