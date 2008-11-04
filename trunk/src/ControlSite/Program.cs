using System;
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

namespace DistDBMS.ControlSite
{
    class Program
    {
        GlobalDataDictionary gdd;

        static void Main(string[] args)
        {
            Program a = new Program();

            a.TestGDD();
            a.TestSQLSyntax();
            a.TestRelationAlgebra();
            
            
        }

        Selection s1, s2; //作为测试使用

        private void TestRelationAlgebra()
        {
            //Sample1
            //关系代数树，示范结构：select * from Course where credit_hour>2 and location='CB‐6'
            Relation r = new Relation();
            r.Type = RelationalType.Selection;
            r.IsDirectTableScheme = true;


            r.DirectTableScheme.Content = "Course";
            r.DirectTableScheme.TableName = "Course";
            r.DirectTableScheme.IsAllFields = true; //因为*
            r.DirectTableScheme.IsDbTable = true;

            r.Predication.Content = "credit_hour>2 and location=\'CB‐6\'";

            string output = (new RelationDebugger()).GetDebugString(r);
            System.Console.WriteLine("\n\nSample1 Relation:");
            System.Console.WriteLine(output);

            //Sample2
            /*
             * select Course.name, Course.credit_hour, Teacher.name
             * from Course, Teacher
             * where Course.teacher_id=Teacher.id and
             * Course.credit_hour>2 and
             * Teacher.title=3
             */
            r = new Relation();
            r.Type = RelationalType.Projection;
            r.IsDirectTableScheme = true; 
            
            Field f1 = new Field();
            f1.AttributeName = "name";
            f1.Content = "Course.name";
            f1.TableName = "Course";
            //做进一步解析后，可以将f1.Table也赋值过去

            Field f2 = new Field();
            f2.AttributeName = "credit_hour";
            f2.Content = "Course.credit_hour";
            f2.TableName = "Course";

            Field f3 = new Field();
            f3.AttributeName = "name";
            f3.Content = "Teacher.name";
            f3.TableName = "Teacher";

            //在Project中的相关属性，表明投影到这些属性的空间上
            r.RelativeAttributes.Fields.Add(f1);
            r.RelativeAttributes.Fields.Add(f2);
            r.RelativeAttributes.Fields.Add(f3);

            r.LeftRelation = new Relation();
            //至此完成了Projection 

            //Join :Course.teacher_id=Teacher.id and
            Relation joinRelation = r.LeftRelation;
            joinRelation.Type = RelationalType.Join;
            joinRelation.IsDirectTableScheme = false;

            Field f4 = new Field();
            f4.TableName = "Course";
            f4.AttributeName = "teacher_id";
            f4.Content = "Course.teacher_id";

            Field f5 = new Field();
            f5.TableName = "Teacher";
            f5.AttributeName = "id";
            f5.Content = "Teacher.id";

            //做Join操作，相关属性是Course.teacher_id和Teacher.id
            joinRelation.RelativeAttributes.Fields.Add(f4);
            joinRelation.RelativeAttributes.Fields.Add(f5);

            //左边关联Course表，Course表做了Selection:Course.credit_hour>2
            joinRelation.LeftRelation = new Relation();
            Relation select1 = joinRelation.LeftRelation;
            select1.Type = RelationalType.Selection;
            //谓词
            select1.Predication.Content = "Course.credit_hour>2";
            //关系一个表
            select1.IsDirectTableScheme = true;
            select1.DirectTableScheme.TableName = "Course";
            select1.DirectTableScheme.IsAllFields = true;

            //右边关联Teacher表，Teacher表做了Selection:Teacher.title=3
            joinRelation.RightRelation = new Relation();
            Relation select2 = joinRelation.RightRelation;
            select2.Type = RelationalType.Selection;
            //谓词
            select2.Predication.Content = "Teacher.title=3";
            //关系一个表
            select2.IsDirectTableScheme = true;
            select2.DirectTableScheme.TableName = "Teacher";
            select2.DirectTableScheme.IsAllFields = true;

            output = (new RelationDebugger()).GetDebugString(r);
            System.Console.WriteLine("\n\nSample2 Relation:");
            System.Console.WriteLine(output);
            
        }

        private void TestSQLSyntax()
        {
            //Sample1
            //关系代数树，示范结构：select * from Course where credit_hour>2 and location='CB‐6'
            s1 = new Selection();
            s1.Fields.IsAllFields = true;
            s1.Fields.TableName = "Course";

            TableScheme table = new TableScheme();
            table.TableName = "Course";
            table.IsAllFields = true;
            s1.Sources.Add(table);
            s1.Condition = new Condition();
            s1.Condition.Operator = RelationOperator.And;

            //左条件
            s1.Condition.LeftCondition = new Condition();
            s1.Condition.LeftCondition.AtomCondition = new AtomCondition();
            s1.Condition.LeftCondition.AtomCondition.Content = "credit_hour>2";
            s1.Condition.LeftCondition.AtomCondition.Operator = LogicOperator.Greater;
            //Course.credit_hour
            s1.Condition.LeftCondition.AtomCondition.LeftOperand.IsValue = false;
            s1.Condition.LeftCondition.AtomCondition.LeftOperand.Field.TableName = "Course";
            s1.Condition.LeftCondition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";
            //2
            s1.Condition.LeftCondition.AtomCondition.RightOperand.IsValue = true;
            s1.Condition.LeftCondition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
            s1.Condition.LeftCondition.AtomCondition.RightOperand.Value = 2;

            //右条件
            s1.Condition.RightCondition = new Condition();
            s1.Condition.RightCondition.AtomCondition = new AtomCondition();
            s1.Condition.RightCondition.AtomCondition.Operator = LogicOperator.Equal;

            s1.Condition.RightCondition.AtomCondition.LeftOperand.IsValue = false;
            s1.Condition.RightCondition.AtomCondition.LeftOperand.Field.TableName = "Course";
            s1.Condition.RightCondition.AtomCondition.LeftOperand.Field.AttributeName = "location";

            s1.Condition.RightCondition.AtomCondition.RightOperand.IsValue = true;
            s1.Condition.RightCondition.AtomCondition.RightOperand.ValueType = AttributeType.String;
            s1.Condition.RightCondition.AtomCondition.RightOperand.Value = "CB‐6";

            System.Console.WriteLine("\n\nSample1 SQL:");
            System.Console.WriteLine(s1.ToString());


            //Sample2
            /*
             * select Course.name, Course.credit_hour, Teacher.name
             * from Course, Teacher
             * where Course.teacher_id=Teacher.id and
             * Course.credit_hour>2 and
             * Teacher.title=3
             */

            s2 = new Selection();
            
            //Select
            s2.Fields.IsAllFields = false;
            s2.Fields.TableName = "";
            Field f1 = new Field(); 
            f1.TableName = "Course";
            f1.AttributeName = "name";
            s2.Fields.Fields.Add(f1); //Course.name

            f1 = new Field();
            f1.TableName = "Course";
            f1.AttributeName = "credit_hour";
            s2.Fields.Fields.Add(f1); //Course.credit_hour

            f1 = new Field();
            f1.TableName = "Teacher";
            f1.AttributeName = "name";
            s2.Fields.Fields.Add(f1); //Teacher.name


            //From Course
            table = new TableScheme();
            table.TableName = "Course";
            table.IsAllFields = true;
            s2.Sources.Add(table);

            //From Teacher
            table = new TableScheme();
            table.TableName = "Teacher";
            table.IsAllFields = true;
            s2.Sources.Add(table);
            s2.Condition = new Condition();
            s2.Condition.Operator = RelationOperator.And;
            //左条件 Course.teacher_id=Teacher.id
            s2.Condition.LeftCondition = new Condition();
            s2.Condition.LeftCondition.Content = "Course.teacher_id=Teacher.id ";
            s2.Condition.LeftCondition.AtomCondition = new AtomCondition();
            s2.Condition.LeftCondition.AtomCondition.Operator = LogicOperator.Equal;

            s2.Condition.LeftCondition.AtomCondition.LeftOperand.IsValue = false;
            s2.Condition.LeftCondition.AtomCondition.LeftOperand.Field.AttributeName = "teacher_id";
            s2.Condition.LeftCondition.AtomCondition.LeftOperand.Field.TableName = "Course";

            s2.Condition.LeftCondition.AtomCondition.RightOperand.IsValue = false;
            s2.Condition.LeftCondition.AtomCondition.RightOperand.Field.AttributeName = "id";
            s2.Condition.LeftCondition.AtomCondition.RightOperand.Field.TableName = "Teacher";

            //右条件 Course.credit_hour>2 and  Teacher.title=3
            s2.Condition.RightCondition = new Condition();
            s2.Condition.RightCondition.Content = "Course.credit_hour>2 and  Teacher.title=3";
            s2.Condition.RightCondition.Operator = RelationOperator.And;

            //右条件的左条件 Course.credit_hour>2
            s2.Condition.RightCondition.LeftCondition = new Condition();
            s2.Condition.RightCondition.LeftCondition.AtomCondition = new AtomCondition();
            s2.Condition.RightCondition.LeftCondition.AtomCondition.Operator = LogicOperator.Greater; //  ">"
            
            //Course.credit_hour
            s2.Condition.RightCondition.LeftCondition.AtomCondition.LeftOperand.IsValue = false;
            s2.Condition.RightCondition.LeftCondition.AtomCondition.LeftOperand.Field.TableName = "Course";
            s2.Condition.RightCondition.LeftCondition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";

            //2
            s2.Condition.RightCondition.LeftCondition.AtomCondition.RightOperand.IsValue = true;
            s2.Condition.RightCondition.LeftCondition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
            s2.Condition.RightCondition.LeftCondition.AtomCondition.RightOperand.Value = 2;

            //右条件的右条件 Teacher.title=3
            s2.Condition.RightCondition.RightCondition = new Condition();
            s2.Condition.RightCondition.RightCondition.AtomCondition = new AtomCondition();
            s2.Condition.RightCondition.RightCondition.AtomCondition.Operator = LogicOperator.Equal; //  "="

            // Teacher.title
            s2.Condition.RightCondition.RightCondition.AtomCondition.LeftOperand.IsValue = false;
            s2.Condition.RightCondition.RightCondition.AtomCondition.LeftOperand.Field.TableName = "Teacher";
            s2.Condition.RightCondition.RightCondition.AtomCondition.LeftOperand.Field.AttributeName = "title";

            //3
            s2.Condition.RightCondition.RightCondition.AtomCondition.RightOperand.IsValue = true;
            s2.Condition.RightCondition.RightCondition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
            s2.Condition.RightCondition.RightCondition.AtomCondition.RightOperand.Value = 3;

            System.Console.WriteLine("\nSample2 SQL:");
            System.Console.WriteLine(s2.ToString());

            //测试
            string[] tests = new string[]{
           "select * from Student",
            "select Course.name from Course",
            "select * from Course where credit_hour>2 and location='CB‐6'",
            "select course_id, mark from Exam",
            "select Course.name, Course.credit_hour, Teacher.name from Course, Teacher where Course.teacher_id=Teacher.id and Course.credit_hour>2 and Teacher.title=3",
            "select Student.name, Exam.mark from Student, Exam where Student.id=Exam.student_id",
            "select Student.id, Student.name, Exam.mark, Course.name from Student, Exam, Course where Student.id=Exam.student_id and Exam.course_id=Course.id and Student.age>26 and Course.location<>'CB‐6'"
            };
            

            ParserSwitcher ps = new ParserSwitcher();
            for (int i = 0; i < tests.Length;i++ )
            {
                bool result = ps.Parse(tests[i]);
                Selection s3 = ps.LastResult as Selection;
                GlobalConsitencyFiller filler = new GlobalConsitencyFiller();
                filler.FillSelection(gdd, s3);



                System.Console.WriteLine("\n\nTEST" + i.ToString() + ":");
                System.Console.WriteLine("Raw: "+tests[i]);
                System.Console.WriteLine("Parse:" + s3.ToString());
            }

            string allocate = "allocate Student.2 to S2";
            bool r = ps.Parse(allocate);
            if (r)
            {
                System.Console.Write("\n\nALLOCATE：" + (ps.LastResult as Allocation).ToString());
            }

            string insert = "insert into Student values (190001, 'xiao ming', 'M', 20, 1)";
            r = ps.Parse(insert);
            if (r)
            {
                System.Console.Write("\n\nInsert：" + (ps.LastResult as Insertion).ToString());
            }

            string delete = "delete from Teacher where id>=290000 and title=2";
            r = ps.Parse(delete);
            if (r)
            {
                System.Console.Write("\n\nDelete：" + (ps.LastResult as Deletion).ToString());
            }

            string define = "define site S3 127.0.0.1:2003";
            r = ps.Parse(define);
            if (r)
            {
                System.Console.Write("\n\nSite Define：" + (ps.LastResult as SiteDefinition).ToString());
            }

            string create = "create table Student (id int key, name char(25), sex char(1), age int, degree int)";
            r = ps.Parse(create);
            if (r)
            {
                System.Console.Write("\n\nTable Create：" + (ps.LastResult as TableCreation).ToString());
            }

            string hfrag = "fragment Teacher horizontally into id<201000 and title<>3, id<201000 and title=3, id>=201000 and title<>3, id>=201000 and title=3";
            r = ps.Parse(hfrag);
            if (r)
            {
                System.Console.Write("\n\nfragment：" + (ps.LastResult as HFragmentation).ToString());
            }

            
            string vfrag = "fragment Course vertically into (id, name), (id, location, credit_hour, teacher_id)";
            r = ps.Parse(vfrag);
            if (r)
            {
                System.Console.Write("\n\nfragment：" + (ps.LastResult as VFragmentation).ToString());
            }

        }

        /// <summary>
        /// 测试构造数据字典
        /// </summary>
        private void TestGDD()
        {
            GDDCreator gddCreator = new GDDCreator();
            string path = "InitScript.txt";
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
                
                while (!sr.EndOfStream)
                {
                    gddCreator.InsertCommand(sr.ReadLine());
                }
                sr.Close();
                gdd = gddCreator.CreateGDD();

                FragmentFinder finder = new FragmentFinder(gdd);
                Condition condition = new Condition();
                condition.AtomCondition = new AtomCondition();
                condition.AtomCondition.LeftOperand.Field.TableName = "Course";
                condition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";
                condition.AtomCondition.LeftOperand.IsField = true;

                condition.AtomCondition.Operator = LogicOperator.Greater;
                condition.AtomCondition.RightOperand.IsValue = true;
                condition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
                condition.AtomCondition.RightOperand.Value = 2;


                finder.GetMatchFragments(condition);


            }
        }
    }
}
