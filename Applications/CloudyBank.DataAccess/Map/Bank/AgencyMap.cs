using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class AgencyMap : ClassMap<Agency>
    {
        public AgencyMap()
        {
            //Id(x => x.Id);

            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='Agency'"));

            Map(x => x.Address);
            Map(x => x.Lat);
            Map(x => x.Lng);
            Map(x => x.OpeningHour);
            Map(x => x.ClosingHour);
        }
    }
}
