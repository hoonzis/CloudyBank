using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class PaymentEventMap : ClassMap<PaymentEvent>
    {
        public PaymentEventMap()
        {
            //Id(x => x.Id);

            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='PaymentEvent'"));

            Map(x => x.Date);
            Map(x => x.Name);
            Map(x => x.Description);
            Map(x => x.PartnerIban).Nullable();
            Map(x => x.Payed);
            Map(x => x.Amount);
            Map(x => x.Regular);

            References(x => x.Operation).Nullable();
            References(x => x.Customer);
            References(x => x.Account).Nullable();
            References(x => x.Partner).Nullable();
        }
    }
}
