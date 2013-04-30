using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Dto;
using CloudyBank.Core.DataAccess;
using CloudyBank.Core.Dto;
using CloudyBank.CoreDomain.Customers;
using System.Transactions;
using Common.Logging;

namespace CloudyBank.Services
{

    public class PaymentEventServices : IPaymentEventServices
    {
        private IRepository _repository;
        private IPaymentEventRepository _paymentEventRepository;
        private IDtoCreator<PaymentEvent, PaymentEventDto> _dtoCreator;
        private readonly ILog log = LogManager.GetLogger(typeof(PaymentEventServices));
        
        public PaymentEventServices(IRepository repository,IPaymentEventRepository paymentEventRepository, IDtoCreator<PaymentEvent, PaymentEventDto> dtoCreator)
        {
            _repository = repository;
            _paymentEventRepository = paymentEventRepository;
            _dtoCreator = dtoCreator;
        }

        public List<PaymentEventDto> GetPaymentEventsForCustomer(int customerID)
        {
            var payments = _paymentEventRepository.GetPaymentEventsForCustomer(customerID);
            if (payments == null)
            {
                return null;
            }
            return payments.Select(x => _dtoCreator.Create(x)).ToList();
        }

        public void UpdatePaymentEvent(ref PaymentEvent paymentEvent, PaymentEventDto paymentDto)
        {
            paymentEvent.Description = paymentDto.Description;
            paymentEvent.Name = paymentDto.Title;
            paymentEvent.PartnerIban = paymentDto.PartnerIban;
            paymentEvent.Amount = paymentDto.Amount;
            paymentEvent.Date = paymentDto.Date;
            paymentEvent.Regular = paymentDto.Regular;

            if (paymentDto.PartnerId != -1)
            {
                BusinessPartner partner = _repository.Load<BusinessPartner>(paymentDto.PartnerId);
                paymentEvent.Partner = partner;
            }

            if (paymentDto.AccountId != -1)
            {
                Account account = _repository.Load<Account>(paymentDto.AccountId);
                paymentEvent.Account = account;
            }
            
        }


        public int CreatePaymentEvent(PaymentEventDto paymentDto, int customerId)
        {
            
            PaymentEvent paymentEvent = new PaymentEvent();

            UpdatePaymentEvent(ref paymentEvent, paymentDto);

            Customer customer = _repository.Load<Customer>(customerId);
            customer.PaymentEvents.Add(paymentEvent);

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    _repository.Save<PaymentEvent>(paymentEvent);
                    _repository.Update<Customer>(customer);
                    _repository.Flush();
                    scope.Complete();
                    return paymentEvent.Id;
                }
                catch (Exception ex)
                {
                    log.Error("Error during Creating new PaymentEvent", ex);
                    return -1;
                }
            }
        }

        

        public PaymentEventDto GetPaymentEventById(int paymentEventId)
        {
            var paymentEvent = _repository.Get<PaymentEvent>(paymentEventId);
            if (paymentEvent != null)
            {
                return _dtoCreator.Create(paymentEvent);
            }
            return null;
        }

        public bool MarkAsPayed(int paymentId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var payment = _repository.Load<PaymentEvent>(paymentId);
                    payment.Payed = true;
                    _repository.Delete(payment);
                    _repository.Flush();
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    log.Error("Error during marking PaymentEvent as payed", ex);
                    return false;
                }
            }
        }


        public bool RemovePayment(int paymentId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var payment = _repository.Load<PaymentEvent>(paymentId);
                _repository.Delete(payment);
                _repository.Flush();
                scope.Complete();
                return true;
            }
        }


        public bool UpdatePaymentEvent(PaymentEventDto paymentDto)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    var payment = _repository.Load<PaymentEvent>(paymentDto.Id);
                    UpdatePaymentEvent(ref payment, paymentDto);
                    _repository.Update<PaymentEvent>(payment);
                    _repository.Flush();
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    log.Error("Error during marking PaymentEvent update", ex);
                    return false;
                }
            }
        }
    }
}
