using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.Core.DataAccess
{
    public interface IAdvisorRepository
    {
        /// <summary>
        /// Returns an advisor identified by its identity (login)
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        Advisor GetAdvisorByIdentity(string identity);

    }
}
