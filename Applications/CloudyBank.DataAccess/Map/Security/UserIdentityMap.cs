using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain;
using CloudyBank.CoreDomain.Security;
using CloudyBank.DataAccess.UserTypes;
using CloudyBank.DataAccess.Configuration;

namespace CloudyBank.DataAccess.Map.Bank
{
    public sealed class UserIdentityMap : ClassMap<UserIdentity>
    {
        public UserIdentityMap()
        {
            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "1000")
                    .AddParam("where", "TableKey='UserIdentity'"));
            Map(x => x.Identification);
            Map(x => x.Type);
            Map(x => x.ValidityEndDate);
            Map(x => x.ValidityStartDate);
            Map(x => x.Email).CustomType<EncryptedString>();
            Map(x => x.UserType);
        }
    }
}
