using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.DataAccess;

namespace CloudyBank.DataAccess.Repository
{
    public class OAuthRepository : BaseRepository, IOAuthRepository
    {
        public OAuthRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public AuthToken GetToken(String token)
        {
            ISession session = SessionFactory.GetCurrentSession();
            var authToken = session.Query<AuthToken>().SingleOrDefault(x => x.Token == token);
            return authToken;
        }

        public AuthConsumer GetConsumer(String key)
        {
            ISession session = SessionFactory.GetCurrentSession();
            var consumer = session.Query<AuthConsumer>().SingleOrDefault(x => x.ConsumerKey == key);
            return consumer;
        }
    }
}
