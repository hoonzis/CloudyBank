using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using CloudyBank.Services.Technical;
using Microsoft.Pex.Framework.Generated;

namespace CloudyBank.UnitTests
{


    [TestClass()]
    [PexClass(typeof(Utils))]
    public partial class UtilsTest
    {

        [PexMethod]
        [PexArguments("http://somereal.web/actual/getAccount")]
        public String GetStringAfterLastSlashTest(string url)
        {
            var actual = Utils.GetStringAfterLastSlash(url);
            return actual;
        }
       
    }
}
