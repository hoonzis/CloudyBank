using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.Web.Ria.ViewModels;
using CloudyBank.TestFixtures.ServiceWrappers;
using System.Threading;

namespace CloudyBank.TestFixtures.Fixtures
{
    //Methods in this fixture class are called by FitNesse.
    public class CustomerFixture
    {
        public CustomerFixture()
        {
            ServicesFactory.Creator = new ServiceForFixturesCreator();
            App.InitApp();
            App.InitSessionFactory();
            App.Bind();
            _customerServices = App.GetObject<ICustomerServices>("CustomerServices");
            _partnerServices = App.GetObject<IBusinessPartnerServices>("BusinessPartnerServices");
        }
        
        private ICustomerServices _customerServices;
        private IBusinessPartnerServices _partnerServices;

        public void CreateCustomer(String firstName, String lastName, String email, String phone,String code)
        {
            _customerServices.CreateCustomer(firstName, lastName, email, phone,code);
        }

        public void CreateBusinessPartner(String customerCode, String name, String iban)
        {
            var customerVM = GetCustomer(customerCode);
            
            BusinessPartnerViewModel partner = new BusinessPartnerViewModel();
            partner.Title = name;
            partner.Iban = iban;
            //partner.IsNew = true;

            var result = customerVM.UpdatePartner(partner);
            //wait for the asynchronous method to finish
            result.AsyncWaitHandle.WaitOne();
            //wait for the callback to finish
            while (customerVM.InProgress) Thread.Sleep(20);
        }

        public bool HasBusinessPartner(String customerCode,String name, String iban)
        {
            var customerVM = GetCustomer(customerCode);
            
            var result = customerVM.LoadPartners();
            result.AsyncWaitHandle.WaitOne();
            while (customerVM.InProgress) Thread.Sleep(20);
            
            var partner = customerVM.Partners.FirstOrDefault(x => x.Title == name && x.Iban == iban);
            return partner != null;
        }

        public CustomerViewModel GetCustomer(String code)
        {
            var customer = _customerServices.GetCustomerByCode(code);
            CustomerViewModel customerVM = new CustomerViewModel();
            customerVM.PartnerService = new BusinessPartnerServiceWrapper(_partnerServices);
            customerVM.CustomerService = new CustomerServiceWrapper(_customerServices);
            var result = customerVM.GetCustomerByID(customer.Id);

            result.AsyncWaitHandle.WaitOne();
            result.AsyncWaitHandle.Close();
            while (customerVM.InProgress == true) Thread.Sleep(20);

            return customerVM;
        }

        public void WaitUntilFinished<T>(Func<object, IAsyncResult> toCall, Func<bool> predicate, T parameter)
        {
            var result = toCall.Invoke(parameter);
            result.AsyncWaitHandle.WaitOne();
            result.AsyncWaitHandle.Close();

            while (predicate.Invoke()) Thread.Sleep(20);

        }
    }
}
