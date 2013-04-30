using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.CoreDomain.Advisors;


namespace CloudyBank.DataAccess.Repository
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        public TaskRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public List<Task> GetTasksForAdvisor(int advisorID)
        {
            ISession session = SessionFactory.GetCurrentSession();
            return session.Query<Task>().Where(x => x.Advisor.Id == advisorID).ToList();
        }
    }
}
