using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;

namespace CloudyBank.Core.Services
{
    public interface IAgencyServices
    {
        IList<AgencyDto> GetAcencies();
        
    }
}
