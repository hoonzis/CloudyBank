using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='Tag'"));

            Map(x => x.Description);
            Map(x => x.Name);
        }
    }
}
