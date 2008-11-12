using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.Plan;

namespace DistDBMS.ControlSite
{
    public class VirtualInterface2
    {
        public Table ExecuteSQL(string sql)
        {
            Relation r = CreateTempRelation();

            return null;
        }

        /*
        private void TestExecutePlan(Relation r)
        {
            ExecutionRelation exR = new ExecutionRelation(r, "PLAN", -1);
            

            ////////////////生成执行计划//////////////////////////
            PlanCreator creator = new PlanCreator();

            ExecutionPlan plan = creator.CreateGlobalPlan(r, "PLAN");
            System.Console.WriteLine(plan.ToString());

            plans = creator.SplitPlan(plan, gdd);
            Hashtable virInterfaces = new Hashtable();

            VirtualBuffer buffer = new VirtualBuffer();
            foreach (ExecutionPlan p in plans)
            {
                //System.Console.WriteLine("\n\n" + p.ToString() + "\n\n");

                //设置不同的站点
                virInterfaces[p.ExecutionSite.Name] = new VirtualInterface(p.ExecutionSite.Name, buffer);
                (virInterfaces[p.ExecutionSite.Name] as VirtualInterface).ReceiveGdd(gdd);



            }
        }*/

        private Relation CreateTempRelation()
        {
            Relation r = new Relation();
            r.Type = RelationalType.Projection;

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
            Relation join = new Relation();
            join.Type = RelationalType.Join;
            select1.Children.Add(join);

            Field f6 = new Field();
            f6.AttributeName = "id";
            f6.TableName = "Course.1";
            join.RelativeAttributes.Fields.Add(f6);

            f6 = new Field();
            f6.AttributeName = "id";
            f6.TableName = "Course.2.2";
            join.RelativeAttributes.Fields.Add(f6);



            Relation select3 = new Relation();
            select3.Type = RelationalType.Selection;
            select3.IsDirectTableSchema = true;
            select3.DirectTableSchema = new TableSchema();
            select3.DirectTableSchema.TableName = "Course.1";
            join.Children.Add(select3);

            select3 = new Relation();
            select3.Type = RelationalType.Selection;
            select3.IsDirectTableSchema = true;
            select3.DirectTableSchema = new TableSchema();
            select3.DirectTableSchema.TableName = "Course.2.2";
            join.Children.Add(select3);


            //右边关联Teacher表，Teacher表做了Selection:Teacher.title=3
            joinRelation.RightRelation = new Relation();
            Relation select2 = joinRelation.RightRelation;
            select2.Type = RelationalType.Selection;
            //谓词
            select2.Predication.Content = "Teacher.title=3";
            //关系一个表
            //select2.IsDirectTableSchema = true;
            //select2.DirectTableSchema.TableName = "Teacher";
            //select2.DirectTableSchema.IsAllFields = true;
            Relation union = new Relation();
            select2.Children.Add(union);
            union.Type = RelationalType.Union;

            Relation union1 = new Relation();
            union1.Type = RelationalType.Selection;
            union1.IsDirectTableSchema = true;
            union1.DirectTableSchema = new TableSchema();
            union1.DirectTableSchema.TableName = "Teacher.2";
            union1.DirectTableSchema.IsAllFields = true;
            union.Children.Add(union1);
            union1 = new Relation();
            union1.Type = RelationalType.Selection;
            union1.IsDirectTableSchema = true;
            union1.DirectTableSchema = new TableSchema();
            union1.DirectTableSchema.TableName = "Teacher.4";
            union1.DirectTableSchema.IsAllFields = true;
            union.Children.Add(union1);

            return r;
        }
    }
}
