using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace CloudyBank.DataAccess.Repository
{
    public class BaseRepository
    {
        protected ISessionFactory SessionFactory { get; private set; }

        public BaseRepository(ISessionFactory sessionFactory)
        {
            this.SessionFactory = sessionFactory;
        }
    }
}
