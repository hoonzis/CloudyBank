using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CloudyBank.DataAccess.Security
{
    public static class DesEncryptionProvider
    {
        private static DESCryptoServiceProvider _des = new DESCryptoServiceProvider();
        static DesEncryptionProvider()
        {
            String key = "MySecret";
            String iv = "MyIVMyIV";
            
            //Set secret key For DES algorithm.
            _des.Key = ASCIIEncoding.ASCII.GetBytes(key);

            //Set initialization vector.
            _des.IV = ASCIIEncoding.ASCII.GetBytes(iv);
        }

        public static string Encrypt(string plain)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(plain);
            ICryptoTransform encryptor = null;
            MemoryStream ms = null;
            try
            {
                encryptor = _des.CreateEncryptor();
                ms = new MemoryStream();
                using (CryptoStream stream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.FlushFinalBlock();
                    ms.Close();
                    byte[] result = ms.ToArray();
                    var cypher = Convert.ToBase64String(result);
                    return cypher;
                }
            }
            finally
            {
                if (encryptor != null)
                    encryptor.Dispose();
                if (ms != null)
                    ms.Dispose();
            }
            
        }

        public static String Decrypt(string cypher)
        {
            byte[] buffer = Convert.FromBase64String(cypher);
            MemoryStream ms = null;
            ICryptoTransform transform = null;
            CryptoStream stream = null;

            try
            {
                ms = new MemoryStream(buffer, 0, buffer.Length);
                transform = _des.CreateDecryptor();
                stream = new CryptoStream(ms, transform, CryptoStreamMode.Read);

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var decrypted = reader.ReadToEnd();
                    return decrypted;
                }

            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
                if (transform != null)
                    transform.Dispose();
                if (ms != null)
                    ms.Dispose();
            }

            //Rather using try-finally block to be sure that some of the streams are not disposed several times
            //which could happen in the following case:


            //using (MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length))
            //{
            //    using (ICryptoTransform transform = _des.CreateDecryptor())
            //    {
            //        using (CryptoStream stream = new CryptoStream(ms, transform, CryptoStreamMode.Read))
            //        {
            //            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            //            {
            //                var decrypted = reader.ReadToEnd();
            //                return decrypted;
            //            }
            //        }
            //    }
            //}
        }
    }
}
