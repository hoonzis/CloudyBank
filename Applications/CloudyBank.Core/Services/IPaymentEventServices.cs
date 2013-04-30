using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using System.Diagnostics.Contracts;

namespace CloudyBank.Core.Services
{
    [ContractClass(typeof(PaymenEventServicesContracts))]
    public interface IPaymentEventServices
    {
        List<PaymentEventDto> GetPaymentEventsForCustomer(int customerId);

        int CreatePaymentEvent(PaymentEventDto paymentDto, int customerId);

        PaymentEventDto GetPaymentEventById(int paymentEventId);

        bool MarkAsPayed(int paymentEventId);

        bool RemovePayment(int paymentId);

        bool UpdatePaymentEvent(PaymentEventDto payment);
        
    }

    [ContractClassFor(typeof(IPaymentEventServices))]
    public abstract class PaymenEventServicesContracts : IPaymentEventServices
    {

        public List<PaymentEventDto> GetPaymentEventsForCustomer(int customerID)
        {
            throw new NotImplementedException();
        }

        public int CreatePaymentEvent(PaymentEventDto payment, int customerID)
        {
            Contract.Requires<PaymentServicesException>(payment != null);
            return -1;
        }

        public PaymentEventDto  GetPaymentEventById(int paymentEventId)
        {
 	        throw new NotImplementedException();
        }

        public bool  MarkAsPayed(int paymentEventId)
        {
 	        throw new NotImplementedException();
        }


        public bool RemovePayment(int paymentId)
        {
            throw new NotImplementedException();
        }


        public bool UpdatePaymentEvent(PaymentEventDto payment)
        {
            Contract.Requires<PaymentServicesException>(payment != null);
            return default(bool);
        }
    }
}
