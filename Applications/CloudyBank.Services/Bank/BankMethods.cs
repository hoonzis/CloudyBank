using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Security;
using System.Text.RegularExpressions;

namespace CloudyBank.Services.Bank
{
    public static class BankMethods
    {
        private static Random random = new Random();

        public static String GenerateAccountCode()
        {
            return String.Empty;
        }

        public static String GenerateCustomerCode(UserType type)
        {
            if (type == UserType.IndividualCustomer)
            {
                return "C0-" + RandomString(4, true) + "-" + RandomString(4, true);
            }
            else if (type == UserType.CorporateCustomer)
            {
                return "C1-" + RandomString(4, true) + "-" + RandomString(4, true);
            }
            else
            {
                throw new Exception("Cannot generate code for unexpected customer type");
            }    
        }

        public static string RandomString(int size, bool upperCase)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(65 + random.Next(26));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string RandomNumberString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(48 + random.Next(9));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
