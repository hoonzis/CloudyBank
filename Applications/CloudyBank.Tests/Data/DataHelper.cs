using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;

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
            customers.Add(new Customer { Id = 3 , Identification = "ident", Code = "code1", Email = "email1" });
            customers.Add(new Customer { Id = 4,Identification = "test", Code = "code2", Email = "email2" });

            return customers;
        }
    }
}
