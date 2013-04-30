using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class SimpleAuditMap : ClassMap<SimpleAudit>
    {
        public SimpleAuditMap()
        {
            Id(x => x.Id);
            Map(x => x.MethodName);
            Map(x => x.CalledAt);
            Map(x => x.CalledBy);
            Map(x => x.ServiceName);
        }
    }
}
