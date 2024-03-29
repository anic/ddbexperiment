﻿using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.RelationalAlgebra;
using DistDBMS.ControlSite.SQLSyntax.Object;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.ControlSite.SQLSyntax.Parser;
using DistDBMS.Common.Syntax;
using DistDBMS.Common;
using System.IO;
using DistDBMS.Common.Dictionary;
using DistDBMS.ControlSite.Finder;
using DistDBMS.ControlSite.SQLSyntax;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.Plan;
using System.Collections;
using System.Threading;
using DistDBMS.ControlSite.RelationalAlgebraUtility;
using DistDBMS.LocalSite;
using DistDBMS.Network;

namespace DistDBMS.ControlSite
{
    class Program
    {
        GlobalDirectory gdd;

        

        static void Main(string[] args)
        {
            #region 测试用代码
            //Program a = new Program();
            //a.TestGDD();
            //a.TestSQLSyntax();
            ////a.TestRelationAlgebra();
            //a.TestExecutionPlan();

            //a.TestGDD();
            //a.TestSQLSyntax();
            //a.TestRelationAlgebra();
            //a.TestExecutionPlan();
            #endregion

            //第一个参数是初始化的脚本
            string scriptFile = "NetworkInitScript.txt";

            if (args.Length >= 1)
                scriptFile = args[0];

            //args.Length >= 1
            NetworkInitiator initiator = new NetworkInitiator();
            ClusterConfiguration clusterConfig = initiator.GetConfiguration(scriptFile);


            //args.Length >=2 
            List<string> controlSite = new List<string>();
            if (args.Length >= 2)
                for (int i = 1; i < args.Length; i++)
                    controlSite.Add(args[i]);
            else //没有参数，启动所有
                controlSite.AddRange(initiator.ControlSiteNames);


            foreach (string site in controlSite)
            {
                ControlSiteServer controlSiteServer = new ControlSiteServer(clusterConfig, site);

                PackageProcessor processor = new PackageProcessor(site, clusterConfig, initiator);
                controlSiteServer.PacketProcessor = new ControlSiteServer.PacketProcessorDelegate(processor.PackageProcess);
                controlSiteServer.Start();

                DistDBMS.Common.Debug.WriteLine("ControlSite " + site + " started!");
            }

            Console.ReadLine();
        }
        #region 测试用代码

//        Relation r;
//        /// <summary>
//        /// 关系代数的填写方法
//        /// </summary>
//        private void TestRelationAlgebra()
//        {
//            //Sample1
//            //关系代数树，示范结构：select * from Course where credit_hour>2 and location='CB‐6'
//            r = new Relation();
//            r.Type = RelationalType.Selection;            
            
//            r.DirectTableSchema.TableName = "Course";
//            r.DirectTableSchema.IsAllFields = true; //因为*
//            r.DirectTableSchema.IsDbTable = true;

//            //r.Predication.Content = "credit_hour>2 and location=\'CB‐6\'";

//            string output = (new RelationDebugger()).GetDebugString(r);
//            DistDBMS.Common.Debug.WriteLine("\n\nSample1 Relation:");
//            DistDBMS.Common.Debug.WriteLine(output);

//            //Sample2
//            /*
//             * select Course.name, Course.credit_hour, Teacher.name
//             * from Course, Teacher
//             * where Course.teacher_id=Teacher.id and
//             * Course.credit_hour>2 and
//             * Teacher.title=3
//             */
//            r = new Relation();
//            r.Type = RelationalType.Projection;
            
//            Field f1 = new Field();
//            f1.AttributeName = "name";
//  //          f1.Content = "Course.name";
//            f1.TableName = "Course";
//            //做进一步解析后，可以将f1.Table也赋值过去

//            Field f2 = new Field();
//            f2.AttributeName = "credit_hour";
////            f2.Content = "Course.credit_hour";
//            f2.TableName = "Course";

//            Field f3 = new Field();
//            f3.AttributeName = "name";
//            //f3.Content = "Teacher.name";
//            f3.TableName = "Teacher";

//            //在Project中的相关属性，表明投影到这些属性的空间上
//            r.RelativeAttributes.Fields.Add(f1);
//            r.RelativeAttributes.Fields.Add(f2);
//            r.RelativeAttributes.Fields.Add(f3);

//            r.LeftRelation = new Relation();
//            //至此完成了Projection 

//            //Join :Course.teacher_id=Teacher.id and
//            Relation joinRelation = r.LeftRelation;
//            joinRelation.Type = RelationalType.Join;
            

//            Field f4 = new Field();
//            f4.TableName = "Course";
//            f4.AttributeName = "teacher_id";
//            //f4.Content = "Course.teacher_id";

//            Field f5 = new Field();
//            f5.TableName = "Teacher";
//            f5.AttributeName = "id";
//            //f5.Content = "Teacher.id";

//            //做Join操作，相关属性是Course.teacher_id和Teacher.id
//            joinRelation.RelativeAttributes.Fields.Add(f4);
//            joinRelation.RelativeAttributes.Fields.Add(f5);

//            //左边关联Course表，Course表做了Selection:Course.credit_hour>2
//            joinRelation.LeftRelation = new Relation();
//            Relation select1 = joinRelation.LeftRelation;
//            select1.Type = RelationalType.Selection;
//            //谓词
//            //select1.Predication.Content = "Course.credit_hour>2";
//            //关系一个表
//            Relation join = new Relation();
//            join.Type = RelationalType.Join;
//            select1.Children.Add(join);

//            Field f6 = new Field();
//            f6.AttributeName = "id";
//            f6.TableName = "Course.1";
//            join.RelativeAttributes.Fields.Add(f6);

//            f6 = new Field();
//            f6.AttributeName = "id";
//            f6.TableName = "Course.2.2";
//            join.RelativeAttributes.Fields.Add(f6);



//            Relation select3 = new Relation();
//            select3.Type = RelationalType.Selection;
            
//            select3.DirectTableSchema = new TableSchema();
//            select3.DirectTableSchema.TableName = "Course.1";
//            join.Children.Add(select3);

//            select3 = new Relation();
//            select3.Type = RelationalType.Selection;
            
//            select3.DirectTableSchema = new TableSchema();
//            select3.DirectTableSchema.TableName = "Course.2.2";
//            join.Children.Add(select3);


//            //右边关联Teacher表，Teacher表做了Selection:Teacher.title=3
//            joinRelation.RightRelation = new Relation();
//            Relation select2 = joinRelation.RightRelation;
//            select2.Type = RelationalType.Selection;
//            //谓词
//            //select2.Predication.Content = "Teacher.title=3";
//            //关系一个表
//            //select2.IsDirectTableSchema = true;
//            //select2.DirectTableSchema.TableName = "Teacher";
//            //select2.DirectTableSchema.IsAllFields = true;
//            Relation union = new Relation();
//            select2.Children.Add(union);
//            union.Type = RelationalType.Union;

//            Relation union1 = new Relation();
//            union1.Type = RelationalType.Selection;
            
//            union1.DirectTableSchema = new TableSchema();
//            union1.DirectTableSchema.TableName = "Teacher.2";
//            union1.DirectTableSchema.IsAllFields = true;
//            union.Children.Add(union1);
//            union1 = new Relation();
//            union1.Type = RelationalType.Selection;
            
//            union1.DirectTableSchema = new TableSchema();
//            union1.DirectTableSchema.TableName = "Teacher.4";
//            union1.DirectTableSchema.IsAllFields = true;
//            union.Children.Add(union1);


//            output = (new RelationDebugger()).GetDebugString(r);
//            DistDBMS.Common.Debug.WriteLine("\n\nSample2 Relation:");
//            DistDBMS.Common.Debug.WriteLine(output);

//        }

//        private void TestExecutionPlan()
//        {
//            int index = 0;
//            ExecutionRelation exR = new ExecutionRelation(r, ref index, -1);
//            string output = (new RelationDebugger()).GetDebugString(exR);
//            DistDBMS.Common.Debug.WriteLine("\n\nSample2 Relation:");
//            DistDBMS.Common.Debug.WriteLine(output);

//            ////////////////生成执行计划//////////////////////////

//            //插入数据
//            DataImporter importer = new DataImporter(gdd);
//            importer.ImportFromFile("Data.txt");

//            ImportPlanCreator importPlanCreator = new ImportPlanCreator(gdd);
//            plans = importPlanCreator.CreatePlans(importer);

//            Hashtable virInterfaces = new Hashtable();
//            VirtualBuffer buffer = new VirtualBuffer();

//            //初始化每个二级接口
//            foreach (Site site in gdd.Sites)
//            {
//                virInterfaces[site.Name] = new VirtualInterface(site.Name, buffer);
//                (virInterfaces[site.Name] as VirtualInterface).ReceiveGdd(gdd);
//            }

//            QueryPlanCreator creator = new QueryPlanCreator(gdd);
//            int id = 0;
//            ExecutionRelation root = new ExecutionRelation(r,ref id,-1);
//            ExecutionPlan plan = creator.CreateGlobalPlan(root, 0);
//            DistDBMS.Common.Debug.WriteLine("\n\n\n*****\n" + plan.ToString() + "\n*****\n");
//            plans = creator.SplitPlan(plan);

//            foreach (ExecutionPlan p in plans)
//                DistDBMS.Common.Debug.WriteLine(p.ToString());

//            #region output
//            /*
//             *  Step 0:
//                PLAN    Projection:  () Attributes: (Course.name, Course.credit_hour, Teacher.na
//                me)
//                PLAN.0  Join:  Attributes: (Course.teacher_id, Teacher.id)
//                PLAN.0.0        Selection:  Predication: Course.credit_hour>2
//                PLAN.0.1        Selection:  Predication: Teacher.title=3
//                Waiting:
//                Transfer:

//                Step 1:
//                PLAN.0.0        Selection:  Predication: Course.credit_hour>2
//                PLAN.0.0.0      Join:  Attributes: (Course.1.id, Course.2.2.id)
//                PLAN.0.0.0.0    Selection:  Course.1()
//                PLAN.0.0.0.1    Selection:  Course.2.2()
//                Waiting:
//                Transfer:

//                Step 2:
//                PLAN.0.1        Selection:  Predication: Teacher.title=3
//                PLAN.0.1.0      Union:
//                Waiting:
//                Transfer:

//                Step 3:
//                PLAN.0.0.0.0    Selection:  Course.1()
//                Waiting:
//                Transfer:

//                Step 4:
//                PLAN.0.0.0.1    Selection:  Course.2.2()
//                Waiting:
//                Transfer:

//                Step 5:
//                PLAN.0.1.0      Union:
//                PLAN.0.1.0.0    Selection:  Teacher.2
//                PLAN.0.1.0.1    Selection:  Teacher.4
//                Waiting:
//                Transfer:

//                Step 6:
//                PLAN.0.1.0.0    Selection:  Teacher.2
//                Waiting:
//                Transfer:

//                Step 7:
//                PLAN.0.1.0.1    Selection:  Teacher.4
//                Waiting:
//                Transfer:
//             */
//            #endregion

//            #region 测试插入的数据
//            //ExecutionPlan insertPlan = new ExecutionPlan();
//            //ExecutionStep insertStep = new ExecutionStep();
//            //insertPlan.Steps.Add(insertStep);
//            //insertStep.Index = 0;
//            //insertStep.Type = ExecutionStep.ExecuteType.Insert;
//            //insertStep.Table = new Table();
//            //Fragment frag = gdd.Fragments.GetFragmentByName("Course.2.2");
//            //insertStep.Table.Schema = frag.Schema.Clone() as TableSchema;
//            //insertStep.Table.Schema.TableName = frag.LogicTable.TableName;
//            //Tuple tuple = new Tuple();
//            //tuple.Data.Add("1");
//            //tuple.Data.Add("CB - 6");
//            //tuple.Data.Add("12");
//            //tuple.Data.Add("4");
//            //insertStep.Table.Tuples.Add(tuple);


//            //DistDBMS.Common.Debug.WriteLine("\n\n" + insertPlan.ToString() + "\n\n");
//            //ExecutionPlan testPlan = plans[3];
//            #endregion


//            foreach (ExecutionPlan p in plans)
//            {
//                ExecutionPackage package = new ExecutionPackage();
//                package.ID = 0;
//                package.Object = p;
//                package.Type = ExecutionPackage.PackageType.Plan;

//                Thread t = new Thread(new ThreadStart(new ThreadExample(virInterfaces[p.ExecutionSite.Name] as VirtualInterface, package).ThreadProc));
//                t.Start();
//            }


//        }

//        internal class ThreadExample
//        {
//            VirtualInterface vInterface;
//            ExecutionPackage package;
//            public ThreadExample(VirtualInterface vInterface,ExecutionPackage package)
//            {
//                this.vInterface = vInterface;
//                this.package = package ;
//            }

//            public void ThreadProc()
//            {
//                vInterface.ReceiveExecutionPackage(package);
//            }
//        }

//        List<ExecutionPlan> plans;
        

//        /// <summary>
//        /// 测试SQL转换
//        /// </summary>
//        private void TestSQLSyntax()
//        {
//            //测试
//            string[] tests = new string[]{
//                /*
//            "select Course.name from Course, Teacher, Student where Course.teacher_id=Teacher.id and Student.id=0 and Course.credit_hour<20 and Course.teacher_id > 12 and Teacher.id>24",
//            "select * from Student",
//            "select Course.name from Course",
//            "select * from Course where credit_hour>2 and location='CB‐6'",
//            "select course_id, mark from Exam",
//            "select Course.name, Course.credit_hour, Teacher.name from Course, Teacher where Course.teacher_id=Teacher.id and Course.credit_hour>2 and Teacher.title=3",
//            "select Student.name, Exam.mark from Student, Exam where Student.id=Exam.student_id",
//                 */
//            "select Student.id, Student.name, Exam.mark, Course.name from Student, Exam, Course where Student.id=Exam.student_id and Exam.course_id=Course.id and Student.age>26 and Course.location<>'CB‐6'"
//            };


//            ParserSwitcher ps = new ParserSwitcher();
//            for (int i = 0; i < tests.Length; i++)
//            {
//                bool result = ps.Parse(tests[i], gdd);
//                Selection s3 = ps.LastResult as Selection;
//                GlobalConsitencyFiller filler = new GlobalConsitencyFiller();
//                //这一步很重要，通过gdd来填写selection的完整信息
//                filler.FillSelection(gdd, s3);

//                //TO 刘璋：可以从这里开始测试转换

//                SQL2RelationalAlgebraInterface converter = new NaiveSQL2RelationalAlgebraConverter();
//                converter.SetQueryCalculus(s3);
//                Relation relationalgebra = converter.SQL2RelationalAlgebra(gdd, true);

//                System.Console.Write(relationalgebra.toString());
//                //QueryOptimizer optimizer = new QueryOptimizer(relationalgebra, gdd);
                
//                /*
//                DistDBMS.Common.Debug.WriteLine("\n\nTEST" + i.ToString() + ":");
//                DistDBMS.Common.Debug.WriteLine("Raw: " + tests[i]);
//                DistDBMS.Common.Debug.WriteLine("Parse:" + s3.ToString());*/

                
//                //System.Console.WriteLine("\n\nTEST" + i.ToString() + ":");
//                //System.Console.WriteLine("Raw: " + tests[i]);
//                //System.Console.WriteLine("Parse:" + s3.ToString());

//                //QueryPlanCreator creator = new QueryPlanCreator(gdd);
//                //ExecutionPlan plan = creator.CreateGlobalPlan(relationalgebra, "PLAN");
//                if (i == 4)
//                {
//                    //string output = (new RelationDebugger()).GetDebugString(relationalgebra);
                    
//                    //DistDBMS.Common.Debug.WriteLine("relationalgebra Parse:\n" + output);
//                    //DistDBMS.Common.Debug.WriteLine("***PLAN**************************\n" + plan.ToString());
//                    //DistDBMS.Common.Debug.WriteLine("***PLAN**************************");
//                    //plans = creator.SplitPlan(plan);
//                    //foreach (ExecutionPlan p in plans)
//                    //    DistDBMS.Common.Debug.WriteLine(p.ToString());

//                    r = relationalgebra;
//                }
                
             
//            }



//        }

//        /// <summary>
//        /// 测试构造数据字典
//        /// </summary>
//        private void TestGDD()
//        {
//            GDDCreator gddCreator = new GDDCreator();
//            string path = "InitScript.txt";
//            if (File.Exists(path))
//            {
//                StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);

//                while (!sr.EndOfStream)
//                {
//                    gddCreator.InsertCommand(sr.ReadLine());
//                }
//                sr.Close();
//                gdd = gddCreator.CreateGDD();

                

//                //FragmentFinder finder = new FragmentFinder(gdd);
//                //Condition condition = new Condition();
//                //condition.AtomCondition = new AtomCondition();
//                //condition.AtomCondition.LeftOperand.Field.TableName = "Course";
//                //condition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";
//                //condition.AtomCondition.LeftOperand.IsField = true;

//                //condition.AtomCondition.Operator = LogicOperator.Greater;
//                //condition.AtomCondition.RightOperand.IsValue = true;
//                //condition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
//                //condition.AtomCondition.RightOperand.Value = 2;

//                //这个类还没完成，可能需要刘璋完成，想用来匹配条件，也可以不用
//                //finder.GetMatchFragments(condition);


//            }
//        }
        #endregion
    }
}
