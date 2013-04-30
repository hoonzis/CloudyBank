using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using FluentNHibernate.Mapping;

namespace CloudyBank.DataAccess.Map.Security
{
    public class AuthConsumerMap : ClassMap<AuthConsumer>
    {
        public AuthConsumerMap()
        {
            Id(x => x.Id);
            Map(x => x.Callback);
            Map(x => x.ConsumerKey);
            Map(x => x.Secret);
            Map(x => x.VerificationCodeFormat);
            Map(x => x.VerificationCodeLength);
            Map(x => x.Name);
            Map(x => x.Description);
        }
    }
}
