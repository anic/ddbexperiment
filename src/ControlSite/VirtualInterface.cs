using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.Common.RelationalAlgebra.Entity;
using DistDBMS.Common.Table;
using DistDBMS.Common.Execution;
using DistDBMS.ControlSite.Plan;
using DistDBMS.Common.Dictionary;
using System.IO;
using System.Collections;
using System.Threading;
using DistDBMS.ControlSite.SQLSyntax.Parser;
using DistDBMS.ControlSite.SQLSyntax.Operation;
using DistDBMS.ControlSite.SQLSyntax;
using DistDBMS.ControlSite.RelationalAlgebraUtility;
using DistDBMS.Common.RelationalAlgebra;

namespace DistDBMS.ControlSite
{
    public class VirtualInterface2
    {
        Hashtable virInterfaces = new Hashtable();
        VirtualBuffer buffer = new VirtualBuffer();
        GlobalDirectory gdd;
        List<ThreadExample> threads = new List<ThreadExample>();

        public bool ExecuteSQL(string sql,out Table data,out string result,out Relation queryTree)
        {
            threads.Clear();
            buffer.Clear();

            ParserSwitcher ps = new ParserSwitcher();
            bool bParse = ps.Parse(sql.Trim());

            if (!bParse)
            {
                data = null;
                result = "Parse sql error. \nInfo:" + ps.LastError;
                queryTree = null;
                return false;
            }

            Selection s3 = ps.LastResult as Selection;
            
            GlobalConsitencyFiller filler = new GlobalConsitencyFiller();
            //这一步很重要，通过gdd来填写selection的完整信息
            bool checkIntergrity = filler.FillSelection(gdd, s3);
            if (!checkIntergrity)
            {
                data = null;
                result = "Syntax error.";
                queryTree = null;
                return false;
            }
            //TO 刘璋：可以从这里开始测试转换

            SQL2RelationalAlgebraInterface converter = new NaiveSQL2RelationalAlgebraConverter();
            converter.SetQueryCalculus(s3);
            Relation relationalgebra = converter.SQL2RelationalAlgebra(gdd, true);

            //TODO 这里作为测试，临时修改，填写ResultName
            TempModifier tempModifier = new TempModifier(gdd);
            tempModifier.Modify(relationalgebra);

            string output = (new RelationDebugger()).GetDebugString(relationalgebra);
            System.Diagnostics.Debug.WriteLine(output);


            List<ExecutionPlan> plans = new List<ExecutionPlan>();

            QueryPlanCreator creator = new QueryPlanCreator(gdd);
            ExecutionPlan gPlan = creator.CreateGlobalPlan(relationalgebra, 0);
            plans = creator.SplitPlan(gPlan);

            foreach (ExecutionPlan p in plans)
            {
                ExecutionPackage package = new ExecutionPackage();
                package.ID = 0;
                package.Object = p;
                package.Type = ExecutionPackage.PackageType.Plan;

                ThreadExample ex = new ThreadExample(virInterfaces[p.ExecutionSite.Name] as VirtualInterface, package);
                Thread t = new Thread(new ThreadStart(ex.ThreadProc));
                threads.Add(ex);
                t.Start();
            }

            Wait(gPlan.Steps[0].Operation.ResultID);

            data = buffer.GetPackageById(gPlan.Steps[0].Operation.ResultID).Object as Table;
            result = "Command executed successfully.";
            result += "\r\n"+data.Tuples.Count+ " tuples selected";
            queryTree = relationalgebra;

            if (data.Tuples.Count > 0)
            {
                result += ":\r\n";
                foreach (Tuple tuple in data.Tuples)
                    result += tuple.ToString() + "\r\n";

            }
            else
                result += "\r\n";
            return true;
        }

        private void Wait(int id)
        {
            while (true)
            {
                //wait for something
                lock (buffer)
                {
                    if (buffer.GetPackageById(id) != null)
                        break;
                }

                Thread.Sleep(1000);
            }
        }

        public bool ImportScript(string filename, out GlobalDirectory outGdd,out string result)
        {
            //初始化GDD
            GDDCreator gddCreator = new GDDCreator();
            string path = filename;
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);

                while (!sr.EndOfStream)
                {
                    gddCreator.InsertCommand(sr.ReadLine());
                }
                sr.Close();
                gdd = gddCreator.CreateGDD();
                

                //初始化每个二级接口
                foreach (Site site in gdd.Sites)
                {
                    virInterfaces[site.Name] = new VirtualInterface(site.Name, buffer);
                    (virInterfaces[site.Name] as VirtualInterface).ReceiveGdd(gdd);
                }

                outGdd = gdd;
                result = "Import script file " + filename + " successfully.";
                return true;
            }
            else
            {
                outGdd = null;
                result = "Fail to import Script file " + filename;
                return false;
            }
            
        }

        public bool ImportData(string filename,out string result)
        {
            threads.Clear();

            DataImporter importer = new DataImporter(gdd);
            importer.ImportFromFile(filename);

            ImportPlanCreator importPlanCreator = new ImportPlanCreator(gdd);
            List<ExecutionPlan> plans = importPlanCreator.CreatePlans(importer);

            foreach (ExecutionPlan p in plans)
            {
                //设置不同的站点
                ExecutionPackage package = new ExecutionPackage();
                package.Type = ExecutionPackage.PackageType.Plan;
                package.Object = p;

                //同步执行
                (virInterfaces[p.ExecutionSite.Name] as VirtualInterface).ReceiveExecutionPackage(package);
            }

            result = "Data imported successful";
            return true;
        }

        internal class ThreadExample
        {
            VirtualInterface vInterface;
            ExecutionPackage package;
            public bool finished;
            public ThreadExample(VirtualInterface vInterface, ExecutionPackage package)
            {
                this.vInterface = vInterface;
                this.package = package;
                finished = false;
            }

            public void ThreadProc()
            {
                finished = false;
                vInterface.ReceiveExecutionPackage(package);
                finished = true;
            }
        }

        
    }
}
