using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using FluentNHibernate.Mapping;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class CustomerProfileMap : ClassMap<CustomerProfile>
    {
        public CustomerProfileMap()
        {

            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                 x => x.AddParam("table", "NH_HiLo")
                     .AddParam("column", "NextHi")
                     .AddParam("maxLo", "1000")
                     .AddParam("where", "TableKey='CustomerProfile'"));

            Map(x => x.HighAge);
            Map(x => x.LowAge);
            Map(x => x.Situation);
            HasMany(x => x.TagsRepartition);
            HasMany(x => x.Customers);
        }
    }
}
