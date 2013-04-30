using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using FluentNHibernate.Mapping;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class BusinessPartnerMap : ClassMap<BusinessPartner>
    {
        public BusinessPartnerMap()
        {
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='BusinessPartner'"));

            Id(x => x.Id);
            Map(x => x.Iban);
            Map(x => x.Description);
            Map(x => x.Name);
            References(x => x.Customer).Nullable();
        }
    }
}
