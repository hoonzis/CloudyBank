using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using NHibernate;
using CloudyBank.CoreDomain.Bank;
using NHibernate.Linq;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.DataAccess.Repository
{
    public class AdvisorRepository :BaseRepository, IAdvisorRepository
    {
        public AdvisorRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
        
        public Advisor GetAdvisorByIdentity(string identity)
        {
            return SessionFactory.GetCurrentSession().Query<Advisor>().SingleOrDefault(x => x.Identification == identity);
        }
    }
}
