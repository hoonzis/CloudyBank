using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public sealed class BalancePointMap : ClassMap<BalancePoint>
    {
        public BalancePointMap()
        {
            //Id(x => x.Id);
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='BalancePoint'"));

            Map(x => x.Balance);
            Map(x => x.Date);
            References(x => x.Account);
        }
    }
}
