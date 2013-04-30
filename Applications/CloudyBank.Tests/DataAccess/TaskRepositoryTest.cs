using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudyBank.UnitTests.TestHelper;
using CloudyBank.Core.DataAccess;
using CloudyBank.DataAccess.Repository;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;


namespace CloudyBank.UnitTests.DataAccess
{
    [TestClass]
    public class TaskRepositoryTest : DataAccessTestBase
    {
        [TestInitialize]
        public void SetUp()
        {
            MyTestInitialize();
        }

        [TestMethod]
        public void GetTaskForAdvisor_Found()
        {
            ITaskRepository taskRepository = new TaskRepository(NhibernateHelper.SessionFactory);
            Repository repository = new Repository(NhibernateHelper.SessionFactory);

            Advisor advisor = new Advisor { Id =1, FirstName="FirstName"};
            Task task = new Task { Id = 1, Advisor = advisor };
            Task task1 = new Task { Id = 2, Advisor = advisor };

            using (NhibernateHelper.SessionFactory.GetCurrentSession().BeginTransaction())
            {
                repository.Save(advisor);
                repository.Save(task);
                repository.Save(task1);

                repository.Flush();

                IList<Task> tasks = taskRepository.GetTasksForAdvisor(advisor.Id);
                Assert.AreEqual(2, tasks.Count);
                Assert.AreEqual(tasks[0].Advisor.FirstName, advisor.FirstName);
            }
        }
    }
}
