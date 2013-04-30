using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;
using CloudyBank.Core.Services.Exceptions;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(BusinessPartnerServicesContracts))]
    public interface IBusinessPartnerServices
    {
        List<BusinessPartnerDto> GetBusinessPartnerForCustomer(int customerID);

        int CreateBusinessPartner(BusinessPartnerDto partner, int customerId);

        bool UpdateBusinessPartner(BusinessPartnerDto partner, int customerId);

        bool RemoveBusinessPartner(int partnerId, int customerId);
    }
    
    [ContractClassFor(typeof(IBusinessPartnerServices))]
    public abstract class BusinessPartnerServicesContracts : IBusinessPartnerServices
    {

        public List<BusinessPartnerDto> GetBusinessPartnerForCustomer(int customerID)
        {
            throw new NotImplementedException();
        }


        public int CreateBusinessPartner(BusinessPartnerDto partner, int customerId)
        {
            Contract.Requires<BusinessPartnersServicesException>(partner != null);
            return default(int);
        }

        public bool UpdateBusinessPartner(BusinessPartnerDto partner, int customerId)
        {
            Contract.Requires<BusinessPartnersServicesException>(partner != null);
            return default(bool);
        }

        public bool RemoveBusinessPartner(int  partnerId, int customerId)
        {
            return default(bool);
        }
    }
}
