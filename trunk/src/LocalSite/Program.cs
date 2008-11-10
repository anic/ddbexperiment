using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using DistDBMS.ControlSite.DataAccess;
using System.IO;
using DistDBMS.ControlSite;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;

namespace DistDBMS.LocalSite
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //本不应该依赖
            //GDDCreator gddCreator = new GDDCreator();
            //GlobalDirectory gdd = null;
            //string path = "InitScript.txt";
            //if (File.Exists(path))
            //{
            //    StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);

            //    while (!sr.EndOfStream)
            //    {
            //        gddCreator.InsertCommand(sr.ReadLine());
            //    }
            //    sr.Close();
            //    gdd = gddCreator.CreateGDD();

            //}
            //try
            //{
            //    File.Delete("Default");
            //}
            //catch(Exception ex) { }

            //using (DataAccessor da = new DataAccessor("Default"))
            //{
            //    if (da.CreateTable(gdd.Schemas[0]))
            //    {
            //        Table table = new Table();
            //        table.Schema = gdd.Schemas[0];
            //        Tuple t = new Tuple();
            //        t.Data.Add("1");
            //        t.Data.Add("Liu Zhang");
            //        t.Data.Add("M");
            //        t.Data.Add("22");
            //        t.Data.Add("100");
            //        table.Tuples.Add(t);

            //        t = new Tuple();
            //        t.Data.Add("2");
            //        t.Data.Add("Cheng Yaoan");
            //        t.Data.Add("M");
            //        t.Data.Add("23");
            //        t.Data.Add("99");
            //        table.Tuples.Add(t);


            //        t = new Tuple();
            //        t.Data.Add("3");
            //        t.Data.Add("Wang Xiaoguang");
            //        t.Data.Add("M");
            //        t.Data.Add("22");
            //        t.Data.Add("101");
            //        table.Tuples.Add(t);

            //        da.InsertValues(table);

            //        //da.DropTable(table.Schema.TableName);

            //        string sql = "select * from student where degree >= 100";
            //        Table table2 = da.Select(sql, gdd.Schemas[0]);
                    
            //    }
            //    //create table Student (id int key, name char(25), sex char(1), age int, degree int)
            //}
        }
    }
}
