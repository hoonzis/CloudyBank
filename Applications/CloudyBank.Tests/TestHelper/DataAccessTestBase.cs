using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Context;
using  CloudyBank.DataAccess.Configuration;

namespace  CloudyBank.UnitTests.TestHelper
{
    /// <summary>
    /// Summary description for DataAccessTestBase
    /// </summary>
    [TestClass]
    public class DataAccessTestBase
    {

        [TestInitialize()]
        public void MyTestInitialize()
        {
            CurrentSessionContext.Bind(NhibernateHelper.SessionFactory.OpenSession());
        }
                
        [TestCleanup()]
        public void MyTestCleanup()
        {
            CurrentSessionContext.Unbind(NhibernateHelper.SessionFactory);
        }
    }
}
