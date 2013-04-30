using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.DataAccess.UserTypes;

namespace CloudyBank.DataAccess.Map.Bank
{
    public class CustomerMap : SubclassMap<Customer>
    {
        public CustomerMap()
        {
            Map(x => x.Code).Not.Nullable();
            Map(x => x.PhoneNumber);
            
            HasMany(x => x.Tags);
            HasMany(x => x.PaymentEvents);
            HasMany(x => x.Partners);
            HasMany(x => x.TagDepenses);
            HasMany(x => x.Images);
            HasMany(x => x.Tokens);
            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.BirthDate).Not.Nullable();
            References(x => x.CustomerProfile);
            Map(x => x.Situation);

            Map(x => x.Password).Not.Nullable();
            Map(x => x.PasswordSalt).Not.Nullable();

            HasManyToMany(x => x.RelatedAccounts)
                .AsMap<Account>(x => x.Key)
                .AsTernaryAssociation();
            
            References(x => x.Advisor);
        }
    }
}
