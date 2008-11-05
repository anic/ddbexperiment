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

        /// <summary>
        /// 关系代数的填写方法
        /// </summary>
        private void TestRelationAlgebra()
        {
            //Sample1
            //关系代数树，示范结构：select * from Course where credit_hour>2 and location='CB‐6'
            Relation r = new Relation();
            r.Type = RelationalType.Selection;
            r.IsDirectTableSchema = true;


            r.DirectTableSchema.Content = "Course";
            r.DirectTableSchema.TableName = "Course";
            r.DirectTableSchema.IsAllFields = true; //因为*
            r.DirectTableSchema.IsDbTable = true;

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
            r.IsDirectTableSchema = true; 
            
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
            joinRelation.IsDirectTableSchema = false;

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
            select1.IsDirectTableSchema = true;
            select1.DirectTableSchema.TableName = "Course";
            select1.DirectTableSchema.IsAllFields = true;

            //右边关联Teacher表，Teacher表做了Selection:Teacher.title=3
            joinRelation.RightRelation = new Relation();
            Relation select2 = joinRelation.RightRelation;
            select2.Type = RelationalType.Selection;
            //谓词
            select2.Predication.Content = "Teacher.title=3";
            //关系一个表
            select2.IsDirectTableSchema = true;
            select2.DirectTableSchema.TableName = "Teacher";
            select2.DirectTableSchema.IsAllFields = true;

            output = (new RelationDebugger()).GetDebugString(r);
            System.Console.WriteLine("\n\nSample2 Relation:");
            System.Console.WriteLine(output);
            
        }

        /// <summary>
        /// 测试SQL转换
        /// </summary>
        private void TestSQLSyntax()
        {
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
                //这一步很重要，通过gdd来填写selection的完整信息
                filler.FillSelection(gdd, s3);

                //TO 刘璋：可以从这里开始测试转换




                System.Console.WriteLine("\n\nTEST" + i.ToString() + ":");
                System.Console.WriteLine("Raw: "+tests[i]);
                System.Console.WriteLine("Parse:" + s3.ToString());
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

                //FragmentFinder finder = new FragmentFinder(gdd);
                //Condition condition = new Condition();
                //condition.AtomCondition = new AtomCondition();
                //condition.AtomCondition.LeftOperand.Field.TableName = "Course";
                //condition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";
                //condition.AtomCondition.LeftOperand.IsField = true;

                //condition.AtomCondition.Operator = LogicOperator.Greater;
                //condition.AtomCondition.RightOperand.IsValue = true;
                //condition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
                //condition.AtomCondition.RightOperand.Value = 2;

                //这个类还没完成，可能需要刘璋完成，想用来匹配条件，也可以不用
                //finder.GetMatchFragments(condition);


            }
        }
    }
}
