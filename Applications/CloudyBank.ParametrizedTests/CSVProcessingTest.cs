// <copyright file="CSVProcessingTest.cs">Copyright ©  2010</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Services.Technical;
using CloudyBank.CoreDomain.Bank;
using Microsoft.Pex.Framework.Generated;

namespace CloudyBank.Services.Technical
{
    /// <summary>This class contains parameterized unit tests for CSVProcessing</summary>
    [PexClass(typeof(CSVProcessing))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CSVProcessingTest
    {
        [PexMethod]
        [PexArguments("10/11/2011", "DESCRIPTION", "-23,30")]
        [PexArguments("10/11/2011", "DESCRIPTION", "-1.235,30")]
        public static Operation ComposeOperation(string timeString, ref string amount, string description)
        {
            var result = CSVProcessing.ComposeOperation(timeString, ref amount, description);
            return result;
        }
       
    }
}
