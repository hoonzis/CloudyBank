using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.DataAccess.Map.Bank
{
    public sealed class AuditMap : ClassMap<Audit>
    {
        public AuditMap()
        {
            Id(x => x.Id);
            Map(x => x.AffectedUser);
            Map(x => x.ChangeTime);
            Map(x => x.NewValue);
            Map(x => x.OldValue);
            Map(x => x.Property);
            Map(x => x.TableName);
        }
    }
}
