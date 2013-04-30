using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Security
{
    public interface IHashProvider
    {
        String CreateSalt(int size);
        String Hash(String input);
    }
}
