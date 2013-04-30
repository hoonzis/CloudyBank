using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.DataAccess.Map.Security
{
    public class AuthTokenMap : ClassMap<AuthToken>
    {
        public AuthTokenMap()
        {
            Id(x => x.Id);
            Map(x => x.Callback);
            References(x => x.AuthConsumer);
            Map(x => x.ExpirationDate);
            Map(x => x.IssueDate);
            Map(x => x.Roles);
            Map(x => x.Scope);
            Map(x => x.State);
            Map(x => x.Token);
            Map(x => x.TokenSecret);
            References(x => x.Customer);
            Map(x => x.VerificationCode);
            Map(x => x.Version);

            //this is slightly against the 1NF, but I will keep it.
            Map(x => x.ScopeName);
        }
    }
}
