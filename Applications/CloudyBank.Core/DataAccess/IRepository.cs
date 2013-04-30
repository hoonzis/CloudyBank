using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace CloudyBank.Core.DataAccess
{
    public interface IRepository
    {
        T Load<T>(object id);
        T Get<T>(object id);
        //int Get(int id);
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> matchingCriteria);
        IEnumerable<T> GetAll<T>();
        void Save<T>(T obj);
        void Update<T>(T obj);
        void Delete<T>(T obj);
        void Flush();
        int CountAll<T>();       
        void Evict<T>(T obj);
        void Refresh<T>(T obj);
        void Clear();
        void SaveOrUpdate<T>(T obj);
    }
}

