using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Customers;
using NHibernate;
using NHibernate.Linq;

namespace CloudyBank.DataAccess.Repository
{
    public class ImageRepository : BaseRepository, IImageRepository
    {
        public ImageRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public List<CustomerImage> GetImagesForCustomer(int customerID)
        {
            return SessionFactory.GetCurrentSession().Query<CustomerImage>().Where(x=>x.Customer.Id == customerID).ToList();
        }

        public IQueryable<System.Tuple<byte[], string>> GetImagesLabelsTuples()
        {
            var images = SessionFactory.GetCurrentSession().Query<CustomerImage>().Select(x => new System.Tuple<byte[], String>(x.Data, x.Customer.Identification));
            return images;
        }
    }
}
