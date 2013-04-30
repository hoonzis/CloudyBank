using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Customers;

namespace CloudyBank.DataAccess.Map.Customers
{
    public class CustomerImageMap : ClassMap<CustomerImage>
    {
        public CustomerImageMap()
        {
            Id(x => x.Id);
            Map(x => x.Data);
            Map(x => x.Date);
            References(x => x.Customer);
        }
    }
}
