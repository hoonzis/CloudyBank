using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoPoco.Engine;
using AutoPoco;
using CloudyBank.CoreDomain.Security;
using AutoPoco.DataSources;
using CloudyBank.DbTool.DataGeneration;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Bank;

namespace CloudyBank.DbTool.Technical
{
    public static class GenerationFactory
    {
        /// <summary>
        /// Configuration of generation session factory. Each property of each POCO object is configured. Several 
        /// properties are configured to use Sources to generated meaningful values.
        /// Several properties have to be set to Value(null). If not then a empty POCO would be generated. This would cause problems
        /// later while saving to DB using NHibernate, while the generated POCO would not be persisted.
        /// </summary>
        /// <returns></returns>
        public static IGenerationSessionFactory ConfigureFactory()
        {
            IGenerationSessionFactory factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c =>
                {
                    c.UseDefaultConventions();
                });

                x.AddFromAssemblyContainingType<UserIdentity>();
                x.Include<UserIdentity>()
                    .Setup(u => u.Email).Use<EmailAddressSource>()
                    .Setup(u => u.ValidityEndDate).Use<ValidityEndDateSource>()
                    .Setup(u => u.ValidityStartDate).Use<ValidityStartDateSource>();

                x.AddFromAssemblyContainingType<Customer>();
                x.Include<Customer>()
                    .Setup(u => u.PhoneNumber).Use<PhoneNumberSource>()
                    .Setup(c => c.FirstName).Use<FirstNameSource>()
                    .Setup(c => c.LastName).Use<LastNameSource>()
                    .Setup(c => c.Code).Use<IndividualCustomerCodeSource>()
                    .Setup(c => c.BirthDate).Use<DateOfBirthSource>()
                    .Setup(c => c.CustomerProfile).Value(null)
                    .Setup(c => c.Situation).Use<RandomEnumerationDataSource<FamilySituation>>()
                    .Setup(u => u.Advisor).Value(null);
                
                x.AddFromAssemblyContainingType<Account>();
                x.Include<Account>()
                    .Setup(a => a.Name).Use<AccountNameDataSource>()
                    .Setup(a => a.Balance).Use<AccountBalanceDataSource>()
                    .Setup(a => a.Iban).Use<IbanDataSource>()
                    .Setup(a => a.Number).Use<AccountNumberSource>();




                x.AddFromAssemblyContainingType<Operation>();
                x.Include<Operation>()
                    .Setup(o => o.Amount).Use<OperationAmountSource>()
                    .Setup(o => o.Description).Use<DescriptionSource>()
                    .Setup(o => o.Direction).Use<RandomEnumerationDataSource<Direction>>()
                    .Setup(o=>o.Tag).Value(null)
                    .Setup(o => o.Date).Use<OperationDateSource>();

                    

                x.AddFromAssemblyContainingType<PaymentEvent>();
                x.Include<PaymentEvent>()
                    .Setup(o => o.Date).Use<OperationDateSource>()
                    .Setup(o => o.Name).Use<MotifSource>()
                    .Setup(o => o.Description).Use<MotifSource>()
                    .Setup(o => o.Amount).Use<OperationAmountSource>()
                    .Setup(o => o.Operation).Value(null)
                    .Setup(o => o.Partner).Value(null);
                    
                    


                x.AddFromAssemblyContainingType<BusinessPartner>();
                x.Include<BusinessPartner>()
                    .Setup(b => b.Name).Use<CorporateNameDataSource>()
                    //.Setup(b=>b.Description).Use<LoremIpsumSource>()
                    .Setup(b => b.Iban).Use<IbanDataSource>();

                x.AddFromAssemblyContainingType<CustomerProfile>();
                x.Include<CustomerProfile>()
                    .Setup(p => p.LowAge).Use<DecaDataSource>(0)
                    .Setup(p => p.HighAge).Use<DecaDataSource>(10);

                double maxLng = 2.425575;
                double minLng = 2.276573;
                double minLat = 48.817264;
                double maxLat = 48.897678;
                x.AddFromAssemblyContainingType<Agency>();
                x.Include<Agency>()
                    .Setup(a => a.Lat).Use<DoubleFromRangeDataSource>(minLat, maxLat)
                    .Setup(a => a.Lng).Use<DoubleFromRangeDataSource>(minLng, maxLng)
                    .Setup(a => a.Address).Use<CorporateNameDataSource>();
            });
            return factory;
        }
    }
}
