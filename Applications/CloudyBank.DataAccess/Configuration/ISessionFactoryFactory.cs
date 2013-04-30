using NHibernate;

namespace  CloudyBank.DataAccess.Configuration
{
    public interface ISessionFactoryFactory
    {
        ISessionFactory GetSessionFactory();
    }
}
