using ml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ml.Attributes;
using ml.Math;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.UnitTests.Data;
using System.Linq;

namespace CloudyBank.UnitTests
{


    [TestClass()]
    public class ConverterTest
    {

        public void ConvertTestHelper<T>()
        {
            var operations = new List<Operation>();

            IEnumerable<Operation> examples = DataHelper.GetOperations().Where(x => x.Tag != null);

            TypeDescription description = null;
            
            var actual = Converter.Convert<Operation>(examples, description);
            Assert.IsTrue(actual.Item1.Cols > 0);
        }

        [TestMethod()]
        public void ConvertTest()
        {
            ConvertTestHelper<GenericParameterHelper>();
        }
    }
}
