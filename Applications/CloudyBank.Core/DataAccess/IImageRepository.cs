using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.Core.DataAccess
{
    public interface IImageRepository
    {
        List<CustomerImage> GetImagesForCustomer(int customerID);
        IQueryable<Tuple<byte[],String>> GetImagesLabelsTuples();
    }
}
