using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Security
{
    public interface ICryptoProvider
    {
        byte[] Encrypt(byte[] input);
        byte[] Decrypt(byte[] decrypt);
    }
}
