using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.Core.DataAccess;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.DataAccess.Repository;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.UnitTests.DataAccess
{
    [TestClass]
    public class AdvisorRepositoryTest : DataAccessTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            MyTestInitialize();
        }

        [TestMethod]
        public void GetAdvisorByIdentity_Found_NotFound()
        {
            IAdvisorRepository advisorRepository = new AdvisorRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Advisor advisor = new Advisor { Email = "e@e.com", FirstName = "Sim", LastName = "Lehericey", Identification="Ident"};

            Advisor retrievedAdvisor;

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(advisor);
                repository.Flush();
                retrievedAdvisor = advisorRepository.GetAdvisorByIdentity(advisor.Identification);
            }
            Assert.IsNotNull(retrievedAdvisor);
            Assert.AreEqual(advisor.Email,retrievedAdvisor.Email);
        }
    }
}
