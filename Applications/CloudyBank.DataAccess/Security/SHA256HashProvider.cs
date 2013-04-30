using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using CloudyBank.Core.Security;

namespace CloudyBank.DataAccess.Security
{
    public class SHA256HashProvider : IHashProvider
    {
        private static IHashProvider _instance;

        public static IHashProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SHA256HashProvider();
                }
                return _instance;
            }
        }

        public String Hash(String input)
        {
            using (SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider())
            {
                if (input != null)
                {
                    byte[] ascii = System.Text.Encoding.ASCII.GetBytes(input);
                    var value = Encoding.ASCII.GetString(provider.ComputeHash(ascii));
                    return value;
                }
                return null;
            }
        }

        public String CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] buff = new byte[size];
                rng.GetBytes(buff);

                // Return a Base64 string representation of the random number.
                return Convert.ToBase64String(buff);
            }
        }
    }
}
