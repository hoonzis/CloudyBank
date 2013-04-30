using CloudyBank.CoreDomain.Bank;
using FluentNHibernate.Mapping;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.DataAccess.Configuration;


namespace CloudyBank.DataAccess.Map.Bank
{
    public sealed class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            //Id(x => x.Id);

            Id(x => x.Id).GeneratedBy.Custom<UniversalHiloGenerator>(
                x => x.AddParam("table", "NH_HiLo")
                    .AddParam("column", "NextHi")
                    .AddParam("maxLo", "10000")
                    .AddParam("where", "TableKey='Account'"));

            Map(x => x.AuthorizeOverdraft);
            Map(x => x.Balance).Not.Nullable();
            Map(x => x.BalanceDate).Nullable();
            Map(x => x.CreationDate).Nullable();
            Map(x => x.Name);
            Map(x => x.NbOfDaysOverdraft);
            Map(x => x.Number).Not.Nullable();
            Map(x => x.Iban).Not.Nullable();
            Map(x => x.Currency);

            HasMany(x => x.TagDepenses);
            HasMany(x => x.Operations);
            HasMany(x => x.BalancePoints);
                //.AsMap(x => x.Date);

            HasManyToMany(x => x.RelatedCustomers)
                .AsMap<Customer>(x => x.Key)
                .AsTernaryAssociation();
        }

    }
}
