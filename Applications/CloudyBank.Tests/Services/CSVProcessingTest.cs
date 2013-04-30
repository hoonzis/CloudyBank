using CloudyBank.Services.Technical;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using CloudyBank.CoreDomain.Bank;
using System.Collections.Generic;
using System.Linq;
using CloudyBank.Core.DataAccess;
using Rhino.Mocks;

namespace CloudyBank.UnitTests
{


    [TestClass()]
    public class CSVProcessingTest
    {


        private TestContext testContextInstance;

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
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

        

        [TestMethod()]
        public void GetTransactionsFromCSVTest()
        {
            var text = "date,description,amount\n" +
            "25/11/2011,RETRAIT DAB 24/11/11 19H35 169770 BNP PARIBAS PARIS ,\"-60,00\"\n" +
            "25/11/2011,VIREMENT RECU TIERS OCTO TECHNOLOGY VIREMENT SALAIRES NOVEMB ,\"2.023,96\"";

            using(TextReader reader = new StringReader(text)){
                var actual = CSVProcessing.GetTransactionsFromCSV(reader).ToList();

                Assert.IsTrue(actual.Count()==2);
                Assert.AreEqual(actual.First().Amount, 60);
            }
        }

        [TestMethod()]
        public void GetCategorizedTransactionsCreateAndStoreTagsTest()
        {
            var text = "date,description,amount,category\n" +
            "25/11/2011,RETRAIT DAB 24/11/11 19H35 169770 BNP PARIBAS PARIS ,\"-60,00\",withdrawal\n" +
            "25/11/2011,VIREMENT RECU TIERS OCTO TECHNOLOGY VIREMENT SALAIRES NOVEMB ,\"2.023,96\",received payment";
            
            var tagsBag = new Dictionary<String,StandardTag>();
            IRepository repository = MockRepository.GenerateStub<IRepository>();

            using (TextReader reader = new StringReader(text))
            {

                var actual = CSVProcessing.GetCategorizedTransactionsCreateAndStoreTags(reader, repository, tagsBag).ToList();

                Assert.IsTrue(actual.Count() == 2);
                Assert.AreEqual(actual.First().Amount, 60);
                Assert.AreEqual(tagsBag.Count, 2);
            }
        }

        [TestMethod()]
        public void ComposeOperationTest()
        {
            string timeString = "10/11/2011";
            string amount = "-3.456,35";
            string description = "SOME DESCRIPTION - OK";


            var actual = CSVProcessing.ComposeOperation(timeString, ref amount, description);
            Assert.IsTrue(actual.Amount == 3456.35m);
            Assert.IsTrue(actual.Direction == Direction.Debit);
        }

        [TestMethod()]
        [DeploymentItem(@"\Data\transactions.csv", "Data")]
        public void GetCategorizedTransactionsCreateAndStoreTagsTestCSV()
        {
            using (TextReader reader = new StreamReader(@"transactions.csv"))
            {
                IRepository repository = MockRepository.GenerateMock<IRepository>();
                Dictionary<string, StandardTag> tagList = new Dictionary<string, StandardTag>();
                IList<Operation> actual;
                actual = CSVProcessing.GetCategorizedTransactionsCreateAndStoreTags(reader, repository, tagList);
                Assert.IsTrue(actual.Count > 0);
            }
        }

        [TestMethod()]
        [DeploymentItem(@"\Data\transactions.csv", "Data")]
        public void GetTransactionsFromCSVTestCSV()
        {
            using (TextReader reader = new StreamReader(@"transactions.csv"))
            {
                IEnumerable<Operation> actual;
                actual = CSVProcessing.GetTransactionsFromCSV(reader);
                Assert.IsTrue(actual.Count() > 0);
            }
        }
    }
}
