using DistDBMS.UserInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DistDBMS.Common.Execution;
using System.IO;
using DistDBMS.Common.Table;
using DistDBMS.LocalSite.DataAccess;

namespace DistDBMS.TestResult
{
    
    
    /// <summary>
    ///This is a test class for FrmAppTest and is intended
    ///to contain all FrmAppTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FrmAppTest
    {


        private TestContext testContextInstance;

        static FrmApp target;

        string[] testsqls = new string[]{
            "select * from Customer",
            "select Product.name from Product",
            "select Product.name from Product where stocks < 4000",
            "select customer_id, number from Purchase where number <= 3",
            "select Product.name,Product.stocks,Producer.name from Product,Producer where Product.producer_id=Producer.id and Producer.location='BJ' and Product.stocks > 4000",
            "select Customer.name,Purchase.number from Customer,Purchase where Customer.id=Purchase.customer_id",
            "select Customer.id,Customer.name,Product.name,Purchase.number from Customer,Product,Purchase where Customer.id=Purchase.customer_id and Product.id=Purchase.product_id and Customer.rank = 1 and Product.stocks > 2000",
            "select * from Product"
        };

        string[] testInsert = new string[]
        {
            "insert into Customer(id, name, gender, rank) values(100010, 'Xiaoming', 'M',1)",
            "insert into Producer(id, name, location) values(200201,'TCL','SH')",
            "delete from Producer where location = 'SH'",
            "delete from Customer where gender = 'M' and rank = 1"
        };


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(@"Start_Sites.bat");
            
            if (!File.Exists(TestDbCreator.FILE_DB_TESTER))
            {
                TestDbCreator creator = new TestDbCreator();
                creator.LoadTestGDD("DbInitScriptTest.txt");
                creator.ImportData("Data.txt");
            }

            target = new FrmApp(); 
            target.RunDefaultWizzard();
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(@"KillAllSites.bat");
            proc.Start();
        }
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ExecuteSQL
        ///</summary>
        [TestMethod()] 
        public void Case0() { BasicCaseTest(0); }

        [TestMethod()]
        public void Case1() { BasicCaseTest(1); }

        [TestMethod()]
        public void Case2() { BasicCaseTest(2); }

        [TestMethod()]
        public void Case3() { BasicCaseTest(3); }

        [TestMethod()]
        public void Case4() { BasicCaseTest(4); }

        [TestMethod()]
        public void Case5() { BasicCaseTest(5); }

        [TestMethod()]
        public void Case6() { BasicCaseTest(6); }

        [TestMethod()]
        public void Case7() { BasicCaseTest(7); }


        public void BasicCaseTest(int i)
        {
            string sql = testsqls[i];
            Table expected;
            ExecutionResult actual;
            using (DataAccessor da = new DataAccessor(TestDbCreator.FILE_DB_TESTER))
            {
                expected = da.Query(sql);
            }
            
            actual = target.ExecuteSQL(sql);
            Assert.AreEqual(expected.Tuples.Count, actual.Data.Tuples.Count, "fail in " + sql);
        
        }

        private bool IsSame(Table r1, Table r2)
        {
           
            if (r1.Tuples.Count != r2.Tuples.Count)
                return false;

            foreach (Tuple t1 in r1.Tuples)
            {
                bool found = false;
                
                foreach (Tuple t2 in r2.Tuples)
                {
                    if (t1.Data.Count > 0 && t2.Data.Count > 0
                        && t1.Data[0] == t2.Data[0])
                    { 
                        //比较
                        bool same = true;
                        for (int i = 0; i < t1.Data.Count; ++i)
                        {
                            if (t1.Data[i] != t2.Data[i])
                            {
                                same = false;
                                break;
                            }
                        }
                        if (same)
                        {
                            found = true;
                            break;
                        }
                    }
                    
                }

                if (!found) //没有相同的
                    return false;

            }

            return true;
        }
        
    }
}
