using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class TaskMap : ClassMap<Task>
    {
        public TaskMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.Descritpion);
            References(x => x.Advisor);
        }
    }
}
