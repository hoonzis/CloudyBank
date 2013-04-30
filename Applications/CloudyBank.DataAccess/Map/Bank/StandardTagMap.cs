using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class StandardTagMap : SubclassMap<StandardTag>
    {
        public StandardTagMap()
        {
            
        }
    }
}
