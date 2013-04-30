using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class AdvisorMap : SubclassMap<Advisor>
    {
        public AdvisorMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            HasMany(x => x.Customers);
        }
    }
}
