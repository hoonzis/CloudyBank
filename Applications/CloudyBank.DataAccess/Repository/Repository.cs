using System;
using System.Collections.Generic;
using System.Linq;
using CloudyBank.Core.DataAccess;
using NHibernate.Criterion;
using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace  CloudyBank.DataAccess.Repository
{
    public class Repository : BaseRepository, IRepository
    {

        public Repository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        #region IRepository<T> Members

        public T Load<T>(object id)
        {
            return SessionFactory.GetCurrentSession().Load<T>(id);
        }

        public T Get<T>(object id)
        {
            return SessionFactory.GetCurrentSession().Get<T>(id);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return SessionFactory.GetCurrentSession().Query<T>();// CreateCriteria(typeof(T)).List<T>();
        }

        public void Update<T>(T obj)
        {
            SessionFactory.GetCurrentSession().Update(obj);
        }

        public void Save<T>(T obj)
        {
            SessionFactory.GetCurrentSession().Save(obj);
        }

        public void Delete<T>(T obj)
        {
            SessionFactory.GetCurrentSession().Delete(obj);
        }

        public void Flush()
        {
            SessionFactory.GetCurrentSession().Flush();
        }

        public void SaveOrUpdate<T>(T obj)
        {
            SessionFactory.GetCurrentSession().SaveOrUpdate(obj);
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> matchingCriteria)
        {
            return SessionFactory.GetCurrentSession().Query<T>().Where(matchingCriteria);
        }

        public int CountAll<T>()
        {
            return SessionFactory.GetCurrentSession().CreateCriteria(typeof(T)).SetProjection(Projections.Count("Id")).UniqueResult<int>();
        }
        
        public void Evict<T>(T obj)
        {
            SessionFactory.GetCurrentSession().Evict(obj);
        }
        public void Refresh<T>(T obj)
        {
            SessionFactory.GetCurrentSession().Refresh(obj);
        }

        public void Clear()
        {
            SessionFactory.GetCurrentSession().Clear();
        }
        
        #endregion


        //public int Get(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
