using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CloudyBank.Dto;
using CloudyBank.Web.Ria;
using CloudyBank.Core.Services;
using System.Windows.Threading;


namespace CloudyBank.TestFixtures.ServiceWrappers
{
    /// <summary>
    /// This is the wrapper to enable the direct connection between the ViewModel and server side business layer service
    /// Only some method are implemented, however it should not be hard to implement all of the methods.
    /// </summary>
    public class CustomerServiceWrapper : CloudyBank.PortableServices.Customers.WCFCustomersService
    {
        public CustomerServiceWrapper()
        {
            
        }

        //server side business layer service
        private ICustomerServices service;

        //delegate to convert the begin-end pattern to single method call
        private Func<int, CloudyBank.PortableServices.Customers.CustomerDto> CustomerByIDDelegate;

        public CustomerServiceWrapper(ICustomerServices service)
        {
            //defining the mapping for AutoMapper
            Mapper.CreateMap<IList<CustomerDto>, IList<CloudyBank.PortableServices.Customers.CustomerDto>>();
            Mapper.CreateMap<CustomerDto, CloudyBank.PortableServices.Customers.CustomerDto>();

            this.service = service;

            //defining the delegate
            this.CustomerByIDDelegate = (i) => {
                App.Bind();
                return Mapper.Map<CustomerDto, CloudyBank.PortableServices.Customers.CustomerDto>(service.GetCustomerById(i));
            };
        }


        public IAsyncResult BeginSaveCustomerDto(PortableServices.Customers.CustomerDto customerDto, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndSaveCustomerDto(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetCustomersForAdvisor(int advisorID, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public List<PortableServices.Customers.CustomerDto> EndGetCustomersForAdvisor(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetCurrentCustomer(AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public PortableServices.Customers.CustomerDto EndGetCurrentCustomer(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetCustomerByID(int id, AsyncCallback callback, object asyncState)
        {

            return CustomerByIDDelegate.BeginInvoke(id, callback, asyncState);
        }

        public PortableServices.Customers.CustomerDto EndGetCustomerByID(IAsyncResult result)
        {
            return CustomerByIDDelegate.EndInvoke(result);
        }
    }
}
