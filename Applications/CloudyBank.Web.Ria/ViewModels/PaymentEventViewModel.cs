using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CloudyBank.MVVM;
using CloudyBank.PortableServices.PaymentEvents;


namespace CloudyBank.Web.Ria.ViewModels
{
    public class PaymentEventViewModel : ViewModelBase
    {
        public bool IsNew { get; set; }

        public PaymentEventViewModel()
        {
            _paymentEvent = new PaymentEventDto();
            IsNew = true;
        }

        public PaymentEventViewModel(PaymentEventDto payment)
        {
            _paymentEvent = payment;
            IsNew = false;
        }

        private PaymentEventDto _paymentEvent;

        public PaymentEventDto Payment
        {
            get { return _paymentEvent; }
            set { 
                _paymentEvent = value;
                OnPropertyChanged(() => Payment);
            }
        }

        public int Id { 
            get { return _paymentEvent.Id; }
            set { 
                _paymentEvent.Id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public String Title
        {
            get { return _paymentEvent.Title; }
            set {
                
                _paymentEvent.Title = value;
                RemoveErrors("Title");
                if (String.IsNullOrWhiteSpace(value))
                {
                    AddError("Title", ViewModelsResources.TitleEmpty);
                }
                OnPropertyChanged(() => Title);
            }
        }

        public String Description
        {
            get { return _paymentEvent.Description; }
            set { 
                _paymentEvent.Description = value;
                OnPropertyChanged(() => Description);
            }
        }

        public decimal Amount { 
            get { return _paymentEvent.Amount;}
            set {
                RemoveErrors("Amount");
                _paymentEvent.Amount = value;
                OnPropertyChanged(() => Amount);
            }
        }

        public DateTime Date
        {
            get { return _paymentEvent.Date; }
            set
            {
                RemoveErrors("Date");
                _paymentEvent.Date = value;
                OnPropertyChanged(() => Date);
            }
        }

        public bool Regular
        {
            get { return _paymentEvent.Regular; }
            set { 
                _paymentEvent.Regular = value;
                OnPropertyChanged(() => Regular);
            }
        }

        public String CreditAccountIban
        {
            get
            {
                if (_partner != null)
                {
                    return _partner.Iban;
                }
                else
                {
                    return _paymentEvent.PartnerIban;
                }
            }

            set
            {
                _paymentEvent.PartnerIban = value;
                OnPropertyChanged(() => CreditAccountIban);
            }
        }


        private AccountViewModel _account;

        public AccountViewModel Account
        {
            get { return _account; }
            set { 
                _account = value;
                if (_account != null)
                {
                    Payment.AccountId = _account.Id;
                }
                else
                {
                    Payment.AccountId = -1;
                }
                OnPropertyChanged(() => Account);
            }
        }

        private BusinessPartnerViewModel _partner;

        public BusinessPartnerViewModel Partner
        {
            get { return _partner; }
            set { 
                _partner = value;
                if (_partner != null)
                {
                    Payment.PartnerId = _partner.Id;
                }
                OnPropertyChanged(() => Partner);
            }
        }
    }
}
