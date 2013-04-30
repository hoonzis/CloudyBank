using System;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using CloudyBank.Core.DataAccess;
using NHibernate.Event;

namespace  CloudyBank.DataAccess.Configuration
{
    public class SessionFactoryFactory : ISessionFactoryFactory
    { 
        private readonly bool _rebuildSchema;
        
        private ISessionFactory _sessionFactory;
        public ISessionFactory GetSessionFactory()
        {
                return _sessionFactory;
        }

        public SessionFactoryFactory(bool rebuildSchema)
        {
            _rebuildSchema = rebuildSchema;
            
            CreateSessionFactoryInstance();
        }

        private void CreateSessionFactoryInstance()
        {
            var config = new NHibernate.Cfg.Configuration();
            //var listener = new AuditEventListener();
            //Here the isteners can be set in order to enable auditing
            //config.SetListener(ListenerType.PreUpdate,listener);
            //config.SetListener(ListenerType.PreInsert, listener);

            try
            {
                _sessionFactory = Fluently.Configure(config)
                    .Mappings(ConfigureMapping)
                    .ExposeConfiguration(NHibernateStuff)
                    .BuildSessionFactory();
            }
            catch (Exception exc)
            {
                throw new DataAccessException(exc.GetBaseException().Message,exc.GetBaseException());
            }
        }

        private static void ConfigureMapping(MappingConfiguration config)
        {
            config.FluentMappings
                //.Conventions.AddFromAssemblyOf<IndexNameConvention>()
                .AddFromAssemblyOf<SessionFactoryFactory>();
        }
        private void NHibernateStuff(NHibernate.Cfg.Configuration config)
        {
            if (_rebuildSchema)
                new SchemaExport(config).Create(_rebuildSchema, true);
        }
    }
}
