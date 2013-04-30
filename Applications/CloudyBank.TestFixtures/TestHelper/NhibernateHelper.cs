using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  CloudyBank.DataAccess.Configuration;
using NHibernate;

namespace  CloudyBank.TestFixtures.TestHelper
{
    internal class NhibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        public static Boolean RebuildSchema = true;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = new SessionFactoryFactory(RebuildSchema).GetSessionFactory();
                }

                return _sessionFactory;
            }
        }
    }
}
