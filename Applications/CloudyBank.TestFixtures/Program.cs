using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Security.Principal;
using System.Web.Security;
using System.Threading;
using CloudyBank.TestFixtures.Fixtures;

namespace CloudyBank.TestFixtures
{
    public static class Program
    {
        public static void Main(String[] args)
        {
            CustomerFixture fixture = new CustomerFixture();
            fixture.CreateCustomer("test", "test", "email", "1433", "code1");


            fixture.CreateBusinessPartner("code1", "test", "IBAN");
            

            var value1 = fixture.HasBusinessPartner("code1", "test", "IBAN");
            Console.WriteLine(value1);
            
            var value = fixture.HasBusinessPartner("code1", "tester", "IBAN1");
            
            Console.Write(value);
            Console.ReadKey();
            //Console.ReadKey();

        }
    }
}
