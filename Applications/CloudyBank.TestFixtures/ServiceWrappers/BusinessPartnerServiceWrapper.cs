using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using AutoMapper;
using CloudyBank.Dto;
using System.Threading;

namespace CloudyBank.TestFixtures.ServiceWrappers
{
    public class BusinessPartnerServiceWrapper : CloudyBank.PortableServices.Partners.WCFPartnerService
    {
        //private object _lock = new object();
        //private  bool _finished;

        private Func<CloudyBank.PortableServices.Partners.BusinessPartnerDto,int, int> _createPartnerDelegate;
        private Func<int, List<CloudyBank.PortableServices.Partners.BusinessPartnerDto>> _partnersByCustomerID;

        private IBusinessPartnerServices _bpServices;

        public BusinessPartnerServiceWrapper(IBusinessPartnerServices bpServices)
        {
            _bpServices = bpServices;
            Mapper.CreateMap<BusinessPartnerDto, CloudyBank.PortableServices.Partners.BusinessPartnerDto>();
            Mapper.CreateMap<CloudyBank.PortableServices.Partners.BusinessPartnerDto, BusinessPartnerDto>();

            _createPartnerDelegate = (partner,customerID) =>
            {
                App.Bind();
                var partnerDTO = Mapper.Map<CloudyBank.PortableServices.Partners.BusinessPartnerDto, BusinessPartnerDto>(partner);
                return _bpServices.CreateBusinessPartner(partnerDTO,customerID);  
            };

            _partnersByCustomerID = (id) =>
            {
                App.Bind();
                var partners = _bpServices.GetBusinessPartnerForCustomer(id);
                return Mapper.Map<IList<BusinessPartnerDto>, List<CloudyBank.PortableServices.Partners.BusinessPartnerDto>>(partners);
            };
        }

        public IAsyncResult BeginGetPartnersForCustomer(int customerID, AsyncCallback callback, object asyncState)
        {
            //lock (_lock)
            //{
            //    _finished = false;
                var result =  _partnersByCustomerID.BeginInvoke(customerID, callback, asyncState);
                result.AsyncWaitHandle.WaitOne();
                return result;
            //}
        }

        public List<PortableServices.Partners.BusinessPartnerDto> EndGetPartnersForCustomer(IAsyncResult result)
        {
            //result.AsyncWaitHandle.WaitOne();
            var partners = _partnersByCustomerID.EndInvoke(result);
            //_finished = true;
            return partners;
            
        }

        public IAsyncResult BeginCreatePartner(PortableServices.Partners.BusinessPartnerDto partnerDto, int customerId, AsyncCallback callback, object asyncState)
        {
          
                //_finished = false;
                return _createPartnerDelegate.BeginInvoke(partnerDto, customerId, callback, null);
          
        }

        public int EndCreatePartner(IAsyncResult result)
        {
                var partnerID = _createPartnerDelegate.EndInvoke(result);
                //_finished = true;
                return partnerID;
          
        }

        public IAsyncResult BeginUpdatePartner(PortableServices.Partners.BusinessPartnerDto partner, int customerId, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public bool EndUpdatePartner(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRemovePartner(int partnerId, int customerId, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public bool EndRemovePartner(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
        
    }
}
