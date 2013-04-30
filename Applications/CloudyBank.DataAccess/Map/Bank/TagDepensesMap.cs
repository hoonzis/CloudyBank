using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class TagDepensesMap : ClassMap<TagDepenses>
    {
        public TagDepensesMap()
        {
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "1000")
                    .AddParam("where", "TableKey='TagDepense'"));

            References(x => x.Tag);
            Map(x => x.Depenses);
        }
    }
}
