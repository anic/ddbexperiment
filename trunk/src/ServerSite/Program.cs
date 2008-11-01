using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ServerSite.RelationalAlgebra.Entity;
using DistDBMS.Common.Entity;
using DistDBMS.ServerSite.RelationalAlgebra;
using DistDBMS.ServerSite.SQLSyntax.Entity;

namespace DistDBMS.ServerSite
{
    class Program
    {
        static void Main(string[] args)
        {
            Program a = new Program();

            a.TestSQLSyntax();
            a.TestRelationAlgebra();
            
        }

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
            System.Console.WriteLine("Sample1:");
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
            System.Console.WriteLine("Sample2:");
            System.Console.WriteLine(output);
            
        }

        private void TestSQLSyntax()
        {
            //Sample1
            //关系代数树，示范结构：select * from Course where credit_hour>2 and location='CB‐6'
            Selection s = new Selection();
            s.Fields.IsAllFields = true;
            s.Fields.TableName = "Course";

            TableScheme table = new TableScheme();
            table.TableName = "Course";
            table.IsAllFields = true;
            s.Sources.Add(table);

            Condition c = new Condition();
            c.IsAtomCondition = false;
            c.Operator = DistDBMS.ServerSite.Common.RelationOperator.And;

            //左条件 And 右条件
            c.LeftCondition = new Condition();
            c.LeftCondition.IsAtomCondition = true;
            c.LeftCondition.AtomCondition.Content = "credit_hour>2";
            c.LeftCondition.AtomCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.Greater;
            //Course.credit_hour
            c.LeftCondition.AtomCondition.LeftOperand = new Operand();
            c.LeftCondition.AtomCondition.LeftOperand.IsValue = false;
            c.LeftCondition.AtomCondition.LeftOperand.Field = new Field();
            c.LeftCondition.AtomCondition.LeftOperand.Field.TableName = "Course";
            c.LeftCondition.AtomCondition.LeftOperand.Field.AttributeName = "credit_hour";
            //2
            c.LeftCondition.AtomCondition.RightOperand = new Operand();
            c.LeftCondition.AtomCondition.RightOperand.IsValue = true;
            c.LeftCondition.AtomCondition.RightOperand.ValueType = AttributeType.Int;
            c.LeftCondition.AtomCondition.RightOperand.Value = 2;

            c.RightCondition = new Condition();
            c.RightCondition.IsAtomCondition = true;
            c.RightCondition.AtomCondition.Operator = DistDBMS.ServerSite.Common.LogicOperator.Equal;

            c.RightCondition.AtomCondition.LeftOperand = new Operand();
            c.RightCondition.AtomCondition.LeftOperand.IsValue = false;
            c.RightCondition.AtomCondition.LeftOperand.Field = new Field();
            c.RightCondition.AtomCondition.LeftOperand.Field.TableName = "Course";
            c.RightCondition.AtomCondition.LeftOperand.Field.AttributeName = "location";

            //2
            c.RightCondition.AtomCondition.RightOperand = new Operand();
            c.RightCondition.AtomCondition.RightOperand.IsValue = true;
            c.RightCondition.AtomCondition.RightOperand.ValueType = AttributeType.String;
            c.RightCondition.AtomCondition.RightOperand.Value = "CB‐6";




        }
    }
}
