using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;


namespace CloudyBank.Services.DataGeneration
{
    public static class GenerationUtils
    {
        private static Random random = new Random();

        public static string RandomString(int size)
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

        public static String GenerateIban(String accountCode, String branchCode, String bankCode, String country)
        {
            String baseCode = bankCode + branchCode + accountCode;
            String compound = baseCode + "0FR00";
            var numbers = ConvertToNumbers(compound);

            var nresult = 98 - BigInteger.Parse(numbers) % 97;
            String result = String.Empty;
            if (nresult < 10)
            {
                result = "0" + nresult;
            }
            else
            {
                result = nresult.ToString();
            }

            if (result.Length == 2)
            {
                return "FR" + result + baseCode;
            }
            else
            {
                return "FR" + result.Substring(0, 1) + baseCode + result[2];
            }
        }

        private static String ConvertToNumbers(string compound)
        {
            var sNewIBAN = String.Empty;
            foreach (var nCharacter in compound)
            {
                if (nCharacter < 48 || nCharacter > 57)
                {
                    // it's a letter, convert it to a number and append
                    //alert ( "Appending " + sIBAN.charAt ( i ) + " as " + String ( nCharacter - 55 ) );
                    sNewIBAN += nCharacter - 55;
                }
                else
                {
                    //alert ( "Appending " + sIBAN.charAt ( i ) + " as " + String ( nCharacter - 48 ) );
                    sNewIBAN += nCharacter - 48;
                }
            }
            return sNewIBAN;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);

            return result;

        }
    }
}
