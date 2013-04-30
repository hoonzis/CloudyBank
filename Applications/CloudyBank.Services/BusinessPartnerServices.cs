using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.CoreDomain.Customers;
using System.Transactions;
using Common.Logging;
using System.Diagnostics.Contracts;

namespace CloudyBank.Services
{
    class BusinessPartnerServices : IBusinessPartnerServices
    {
        private IRepository _repository;
        private IDtoCreator<BusinessPartner, BusinessPartnerDto> _dtoCreator;
        private readonly ILog _log = LogManager.GetLogger(typeof(BusinessPartnerServices));

        public BusinessPartnerServices(IRepository repository, IDtoCreator<BusinessPartner, BusinessPartnerDto> dtoCreator)
        {
            _repository = repository;
            _dtoCreator = dtoCreator;
        }

        public List<Dto.BusinessPartnerDto> GetBusinessPartnerForCustomer(int customerID)
        {
            var customer = _repository.Load<Customer>(customerID);
            if(customer!=null && customer.Partners!=null)
            {
                return customer.Partners.Select(x=>_dtoCreator.Create(x)).ToList();
            }
            return null;
        }

        public void UpdatePartner(BusinessPartnerDto dto, ref BusinessPartner poco)
        {
            poco.Description = dto.Description;
            poco.Iban = dto.Iban;
            poco.Id = dto.Id;
            poco.Name = dto.Title;
        }

        public int CreateBusinessPartner(BusinessPartnerDto dto, int customerId)
        {
            BusinessPartner partner = new BusinessPartner();
            UpdatePartner(dto, ref partner);

            var customer = _repository.Load<Customer>(customerId);

            using (TransactionScope scope = new TransactionScope())
            {
                int returnVal =-1;
                try
                {
                    customer.Partners.Add(partner);
                    _repository.Save<BusinessPartner>(partner);
                    _repository.Update<Customer>(customer);
                    returnVal = partner.Id;
                    scope.Complete();
                    
                }
                catch (Exception ex)
                {
                    _log.Error("Error during creatin new Business Partner", ex);   
                }
                return returnVal;
            }
        }

        public bool UpdateBusinessPartner(BusinessPartnerDto dto, int customerId)
        {
            BusinessPartner partner = _repository.Load<BusinessPartner>(dto.Id);
            UpdatePartner(dto, ref partner);

            bool returnVal = false;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    _repository.Update<BusinessPartner>(partner);
                    _repository.Flush();
                    returnVal = true;
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _log.Error("Error during updating business partner", ex);
                }
            }
            return returnVal;
        }

        public bool RemoveBusinessPartner(int partnerId, int customerId)
        {
            BusinessPartner partner = _repository.Load<BusinessPartner>(partnerId);
            bool returnVal = false;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    _repository.Delete<BusinessPartner>(partner);
                    _repository.Flush();
                    returnVal = true;
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _log.Error("Error during updating business partner", ex);
                }
            }
            return returnVal;
        }
    }
}
