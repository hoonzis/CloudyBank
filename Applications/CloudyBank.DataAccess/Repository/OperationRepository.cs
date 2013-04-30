using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Bank;
using System;

namespace CloudyBank.DataAccess.Repository
{
    public class OperationRepository : BaseRepository,IOperationRepository
    {
        public OperationRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public IList<Operation> GetOperationsByAccount(int accountId)
        {
            ISession session = SessionFactory.GetCurrentSession();
            IQueryable<Operation> result = (from operation in session.Query<Operation>()
                                          where operation.Account.Id == accountId
                                          select operation);

            return result.ToList();
        }

    }
}
