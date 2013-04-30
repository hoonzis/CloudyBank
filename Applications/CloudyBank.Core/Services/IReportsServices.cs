using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudyBank.Core.Services
{
    public interface IReportsServices
    {
        String GenerateAccountsList(int userId);
    }
}
