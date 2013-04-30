using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.DataAccess.Map.Security
{
    public class AuthNonceMap : ClassMap<AuthNonce>
    {
        public AuthNonceMap()
        {
            Id(x => x.Id);
            Map(x => x.Code);
            Map(x => x.Context);
            Map(x => x.Timestamp);
        }
    }
}
