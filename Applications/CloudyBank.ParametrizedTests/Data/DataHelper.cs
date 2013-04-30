using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.UnitTests.Data
{
    public static class DataHelper
    {
        public static IList<Operation> GetOperations()
        {

            var tags = new List<Tag>();
            tags.Add(new StandardTag { Name = "Supermarket", Description = "toto toto", Id = 1 });
            tags.Add(new StandardTag { Name = "Traveling", Description = "titi", Id = 14 });
            var operations = new List<Operation>();

            operations.Add(new Operation { Amount = 33, Description = "CARTE MONOP", Tag = tags[0] });
            operations.Add(new Operation { Amount = 13, Description = "CARTE MONOP", Tag = tags[0] });
            operations.Add(new Operation { Amount = 24, Description = "CARTE SNCF Paris", Tag = tags[1] });
            operations.Add(new Operation { Amount = 120, Description = "CARTE SNCF", Tag = tags[1] });
            operations.Add(new Operation { Amount = 120, Description = "CARTE SNCF Billet", Tag = tags[1] });
            
            operations.Add(new Operation { Amount = 20, Description = "CARTE MONOP" });

            return operations;
        }

        public static IList<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer { Id = 1, Identification = "Ident", Email = "email", Code="code_s" });
            customers.Add(new Customer { Id = 2, Identification = "id2", Email = "email2", Code = "code2" });
            customers.Add(new Customer { Id = 3 , Identification = "ident", Code = "code3", Email = "email1" });
            customers.Add(new Customer { Id = 4,Identification = "test", Code = "code4", Email = "email2" });

            return customers;
        }

        public static IList<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(new Task { Descritpion = "Description", Id = 1 });
            tasks.Add(new Task { Descritpion = "Des", Id = 2 });
            return tasks;
        }

        public static IList<Advisor> GetAdvisors()
        {
            List<Advisor> advisors = new List<Advisor>();
            advisors.Add(new Advisor { FirstName = "Peter", LastName = "Advisor", Id = 1, Tasks = GetTasks()});
            advisors[0].Tasks[0].Advisor = advisors[0];
            advisors[0].Tasks[1].Advisor = advisors[0];
            advisors.Add(new Advisor { FirstName = "John", LastName = "Advisor", Id = 2, Tasks = new List<Task>()});
            
            return advisors;
        }

        public static IList<Account> GetAccounts()
        {
            var accounts = new List<Account>();

            accounts.Add(new Account { Id = 1, Balance = 100, AuthorizeOverdraft = false });
            accounts.Add(new Account { Id = 2, Balance = 50 });
            accounts.Add(new Account { Id = 3, Balance = 10, AuthorizeOverdraft = true });
            return accounts;
        }
    }
}
