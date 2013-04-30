using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class UserTagMap : SubclassMap<UserTag>
    {
        public UserTagMap()
        {
            References(x => x.Customer);
        }
    }
}
