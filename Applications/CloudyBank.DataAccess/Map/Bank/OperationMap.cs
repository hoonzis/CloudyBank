using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using FluentNHibernate.Mapping;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class OperationMap: ClassMap<Operation>
    {
        public OperationMap()
        {
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='Operation'"));// HiLo("NH_HiLo", "NextHi", "1000000", "TableKey=''Operation''");
            References(x => x.Account).Not.Nullable();
            Map(x => x.Amount).Not.Nullable();
            Map(x => x.Direction).Not.Nullable();
            Map(x => x.Motif);
            Map(x => x.Date);
            Map(x => x.TransactionCode).Not.Nullable();
            Map(x => x.Currency);
            Map(x => x.Description);
            References(x => x.Tag);
        }
    }
}
