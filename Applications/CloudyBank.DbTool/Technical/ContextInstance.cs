using NHibernate;
using NHibernate.Context;
using Spring.Context;
using Spring.Context.Support;

namespace CloudyBank.DbTool.Technical
{
    public static class ContextInstance
    {
        private static IApplicationContext _applicationContext;
        private static readonly object _applicationContextSyncRoot = new object();
        private static ISessionFactory _sessionFactory;

        public static void Create()
        {
            if (_applicationContext == null)
            {
                lock (_applicationContextSyncRoot)
                {
                    if (_applicationContext == null)
                    {
                        _applicationContext = ContextRegistry.GetContext();
                    }
                }
            }

            _sessionFactory = (ISessionFactory)_applicationContext.GetObject("SessionFactory");
            if (!(CurrentSessionContext.HasBind(_sessionFactory)))
                CurrentSessionContext.Bind(_sessionFactory.OpenSession());

        }
    }
}
