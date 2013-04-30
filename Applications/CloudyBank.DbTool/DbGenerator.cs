using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using AutoPoco;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.DataAccess.Configuration;
using CloudyBank.DataAccess.Repository;
using CloudyBank.DbTool.DataGeneration;
using CloudyBank.DbTool.Technical;
using CloudyBank.DataAccess.Security;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive;
using CloudyBank.Services.Technical;
using CloudyBank.Services.Aggregations;
using CloudyBank.Services.DataGeneration;

namespace CloudyBank.DbTool
{
    public static class DbGenerator
    {
        #region Consts
        private const String SCRIPTS_DIR = "./Scripts";
        private const String DB_CREATION_SCRIPT_FILENAME = "Database_Creation.sql";
        private const String USER_NAME = "OctoBank";
        private const String USER_PASS = "OctoBank";
        private const int CUSTOMERS_COUNT = 30;
        #endregion

        private static IRepository _repository;
        private static IRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = new Repository(SessionFactory);
                }
                return _repository;
            }
        }

        private static StandardTag _emptyTag;
        public static StandardTag EmptyTag
        {
            get
            {
                if (_emptyTag == null)
                {
                    _emptyTag = new StandardTag { Name = "Not set", Description = "Tag for non-clasified transactions" };
                    Repository.Save(_emptyTag);
                }
                return _emptyTag;
            }
        }

        private static Dictionary<String, StandardTag> _tagsBag = new Dictionary<string, StandardTag>();

        private static IList<Operation> _categorizedTransactions;
        
        private static ISessionFactory SessionFactory;

        public static String GenerateDatabase(String dbName)
        {
            return GenerateDatabase(dbName, USER_NAME, Path.Combine(SCRIPTS_DIR, DB_CREATION_SCRIPT_FILENAME),USER_PASS);
        }

        public static String GenerateDatabase(String dbName, String userName, String sqlFilePath, String userPass)
        {
            String returnMessage;

            try
            {
                String dropCreationScript = File.ReadAllText(sqlFilePath, Encoding.Default);
                dropCreationScript = dropCreationScript.Replace("DATABASE_NAME", dbName);
                dropCreationScript = dropCreationScript.Replace("DATABASE_USER", userName);
                dropCreationScript = dropCreationScript.Replace("DATABASE_PASSWORD", userPass);

                String connectionString = ConfigurationManager.ConnectionStrings["Admin"].ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    Server server = new Server(new ServerConnection(connection));
                    server.ConnectionContext.ExecuteNonQuery(dropCreationScript);
                }

                DynamicConnectionProvider.CurrentDatabaseName = dbName;

                returnMessage = String.Format("Database '{0}' has been dropped and created succesfully.", dbName);
            }
            catch (Exception ex)
            {
                returnMessage = String.Format("An error occured while creating/dropping the database '{0}': {1} \n {2}", dbName, ex.Message, ex);
            }

            return returnMessage;
        }

        public static void GenerateSchema(String dbName)
        {

            SessionFactory = new SessionFactoryFactory(true).GetSessionFactory();
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
        }

        public static void GenerateData()
        {
            IGenerationSessionFactory factory = GenerationFactory.ConfigureFactory();

            IGenerationSession session = factory.CreateSession();

            Role[] roles = CreateAndSaveRoles(Repository);
            IList<CustomerProfile> profiles = CreateAndSaveCustomerProfiles(Repository);
            CreateAndSaveAgencies(session, Repository);
            CreateAndSaveOAuthConsumers(Repository);
            CreateCustomersWithAccounts(session, Repository, roles);
            
            Repository.Flush();
        }

        public static void ComputeData(Stopwatch sw)
        {
            sw.Restart();
            var sessFactFactory = new SessionFactoryFactory(false);
            var sessionFactory = sessFactFactory.GetSessionFactory();
            var repository = new Repository(sessionFactory);
            repository.Clear();

            AggregationServices services = new AggregationServices(repository);

            services.ReAggregateAllOperations();
            sw.Stop();
            Console.WriteLine("Profiles computed in: {0}", sw.ElapsedMilliseconds);
            sw.Restart();
            services.ComputeBalancesForAllAccounts();

            repository.Flush();
            sw.Stop();
            Console.WriteLine("Balances computed in: {0}", sw.ElapsedMilliseconds);
        }

        #region Create and Save Methods

        private static IList<CustomerProfile> CreateAndSaveCustomerProfiles(IRepository repository)
        {
            IList<CustomerProfile> profiles = new List<CustomerProfile>();
            FamilySituation[] situations = (FamilySituation[])Enum.GetValues(typeof(FamilySituation));
            foreach (var situation in situations)
            {
                for (int i = 2; i < 6; i++)
                {
                    int lowAge = i * 10;
                    int highAge = lowAge + 10;
                    CustomerProfile profile = new CustomerProfile();
                    profile.Situation = situation;
                    profile.LowAge = lowAge;
                    profile.HighAge = highAge;
                    repository.Save(profile);
                    profiles.Add(profile);
                }
                CustomerProfile profileExtra = new CustomerProfile { LowAge = 60, HighAge = 80, Situation = situation };
                repository.Save(profileExtra);
                profiles.Add(profileExtra);
            }

            return profiles;
        }

        private static void CreateAndSaveAgencies(IGenerationSession session, IRepository repository)
        {
            session.List<Agency>(10)
                .Impose(x=>x.ClosingHour,DateTime.Now.Date.AddHours(19))
                .Impose(x=>x.OpeningHour,DateTime.Now.Date.AddHours(8))
                .Get()
                .ForEach(x => repository.Save(x));
        }

        private static void AddAccountsAndOperations(Customer customer, Role[] roles, IRepository repository, IGenerationSession session)
        {
            //get the transaction data from csv file
            using (var reader = new StreamReader(@"Data\transactions.csv"))
            {
                _categorizedTransactions = CSVProcessing.GetCategorizedTransactionsCreateAndStoreTags(reader, repository, _tagsBag);
            }

            Account account = session.Single<Account>().Get();
            account.Name = "Savings account";
            account.RelatedCustomers.Add(customer, roles[0]);
            account.Iban = GenerationUtils.GenerateIban(account.Number, "12345", "12345", "FR");
            account.Currency = "EUR";


            Account account2 = session.Single<Account>().Get();
            account2.Name = "Checking account";
            account2.RelatedCustomers.Add(customer, roles[1]);
            account2.Currency = "EUR";

            customer.RelatedAccounts.Add(account, roles[0]);
            customer.RelatedAccounts.Add(account2, roles[1]);

            repository.Save(account);
            repository.Save(account2);

            repository.Save(customer);
            
            repository.Flush();

            //Get random transactions from the list
            Random rnd = new Random();
            var randomTransactions = _categorizedTransactions.Where(x => x.Tag.Name != "Not set").OrderBy(x => rnd.Next());
            
            
            //add the first half to the first account
            SelectForAccount(repository, account, rnd, randomTransactions);
            SelectForAccount(repository, account2, rnd, randomTransactions);

            //IList<Operation> operations = session.List<Operation>(20)
            //    .Impose(x => x.TransactionCode, Guid.NewGuid().ToString())
            //    .Impose(x => x.Currency, "EUR")
            //    .Impose(x=>x.Tag,EmptyTag)
            //    .First(10)
            //        .Impose(x => x.Account, account)
            //    .Next(10)
            //        .Impose(x => x.Account, account2)
            //    .All()
            //    .Get();
            
            //operations.ForEach(x => repository.Save(x));

            repository.Flush();

            var paymentEvents = session.List<PaymentEvent>(20)
                .First(10)
                    .Impose(x => x.Account, account)
                    .Impose(x=>x.Customer,customer)
                .Next(10)
                    .Impose(x => x.Account, account2)
                    .Impose(x=>x.Customer, customer)
                .All()
                .Get();

            paymentEvents.ForEach(x => repository.Save(x));

            repository.Flush();
        }

        //adds first 80 transactions with the date originated from the CSV file (to have some historic information)
        //adds next 40 transactions and chooses random date from last 20 days - to have some actual information
        private static void SelectForAccount(IRepository repository, Account account, Random rnd, IOrderedEnumerable<Operation> randomTransactions)
        {
            randomTransactions.Take(80).ForEach(x => { x.Account = account; repository.Save(x); });

            randomTransactions.Skip(80).Take(40).ForEach(x =>
            {
                x.Account = account;
                x.Date = DateTime.Now.Subtract(new TimeSpan(rnd.Next(20), 0, 0, 0));
                repository.Save(x);
            });
        }

        private static void CreateCustomersWithAccounts(IGenerationSession session,IRepository repository, Role[] roles)
        {
            var customers = session.
            List<Customer>(CUSTOMERS_COUNT).Get();

            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].Identification = "00000000" + i;
                customers[i].PasswordSalt = GenerationUtils.RandomString(5);
                customers[i].Password = SHA256HashProvider.Instance.Hash("Test" + customers[i].PasswordSalt);

                repository.Save(customers[i]);

                AddAccountsAndOperations(customers[i], roles, repository, session);

                //create some bussiness partners for each customer
                session.List<BusinessPartner>(5)
                    .Impose(x => x.Customer, customers[i])
                    .Get().ForEach(x => repository.Save(x));
                
            }
            repository.Flush();
        }

        private static Role[] CreateAndSaveRoles(IRepository repository)
        {
            Role[] roles = new Role[4];
            roles[0] = new Role { Name = "Owner", Permission = Permission.Modify | Permission.Read | Permission.Write | Permission.Transfer };
            roles[1] = new Role { Name = "Advisor", Permission = Permission.Modify | Permission.Read | Permission.Write | Permission.Transfer };
            roles[2] = new Role { Name = "OperationTagger", Permission = Permission.Read | Permission.TagOperations };
            roles[3] = new Role { Name = "Minor", Permission = Permission.Read };

            roles.ForEach(x => Repository.Save(x));
            
            repository.Flush();
            repository.Clear();
            return roles;
        }

        private static void CreateAndSaveOAuthConsumers(IRepository repository)
        {
            List<AuthConsumer> authConsumers = new List<AuthConsumer>();
            authConsumers.Add(new AuthConsumer { Name = "Account Analysis Solution", ConsumerKey = "key1", Secret = "secret1", Description = "This application allows you to perform analytics on your accounts data." });
            authConsumers.Add(new AuthConsumer { Name = "Cool PFM application", ConsumerKey = "key2", Secret = "secret2", Description = "Applications which allows to perform advanced analytics, budget management and predictions" });

            authConsumers.ForEach((x) => repository.Save(x));
            repository.Flush();
        }

        #endregion     
    }
}
