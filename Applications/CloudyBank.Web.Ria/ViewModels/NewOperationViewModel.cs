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
using System.ServiceModel;
using System.ServiceModel.Security;
using CloudyBank.Web.Ria.Resources;
using CloudyBank.PortableServices.Operations;
using CloudyBank.PortableServices.PaymentEvents;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class NewOperationViewModel : ViewModelBase
    {

        public NewOperationViewModel()
        {

        }

        
        #region Services
        WCFOperationService _operationService;
        public WCFOperationService OperationService
        {
            get
            {
                if (_operationService == null)
                {
                    _operationService = ServicesFactory.GetObject<WCFOperationService>();//, WCFOperationServiceClient>();
                }

                return _operationService;
            }
            set { _operationService = value; }
        }


        private WCFPaymentEventService _paymentEventService;

        public WCFPaymentEventService PaymentEventService
        {
            get {
                if (_paymentEventService == null)
                {
                    _paymentEventService = new WCFPaymentEventServiceClient();
                }
                return _paymentEventService; 
            }
            
        }
        #endregion

        #region MakeTransfer
        public bool CanMakeTransfer()
        {
            Message = String.Empty;

            if (Amount <= 0)
            {
                Message = ViewModelsResources.AmountNegative;
                return false;
            }
            if (From == null)
            {
                Message = ViewModelsResources.FromAccountNotSet;
                return false;
            }

            if (From.Balance < Amount && !From.Account.AuthorizeOverdraft)
            {
                Message = ViewModelsResources.NotEnoughMoney;
                return false;
            }

            if (To != null && To == From)
            {
                Message = ViewModelsResources.ErrorSameAccounts;
                return false;
            }

            if (ToExternal)
            {
                return CreditAccountIban != null;
            }
            else
            {
                return To != null && To != From;
            }   
        }

        private CommandBase _makeTransferCommand;

        public CommandBase MakeTransferCommand
        {
            get
            {
                if (_makeTransferCommand == null)
                {
                    _makeTransferCommand = new CommandBase(() => MakeTransfer(), () => CanMakeTransfer());
                }
                return _makeTransferCommand;
            }
            set { _makeTransferCommand = value; }
        }

        public void MakeTransfer()
        {
            InProgress = true;
            Message = Messages.ITransferInProgress;
            
            if (PartnerName != null)
            {
                Motif = PartnerName + " - " + Motif;
            }

            if (ToExternal)
            {
                OperationService.BeginMakeTransferToExternal(From.Id, CreditAccountIban, Amount, Motif, EndMakeTransferToExternal, null);
            }
            else
            {
                OperationService.BeginMakeTransfer(From.Id, To.Id, Amount, Motif, EndMakeTransfer, null);
            }   
        }

        private void ShowTransferOK(String transactionCode)
        {
            Message = String.Format(ViewModelsResources.TransactionOK,transactionCode);
        }

        private void ShowSecurityError()
        {
            Message = ViewModelsResources.TransactionSecurityError;
                
        }

        public void EndMakeTransfer(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    string transactionCode = OperationService.EndMakeTransfer(result);
                    NotifyAccountsAboutTransaction(transactionCode);
                    ShowTransferOK(transactionCode);
                }
                catch (Exception)
                {
                    Message = ViewModelsResources.ErrorWhileMakingTransfer;
                }
                finally
                {
                    InProgress = false;
                }
            }
        }

        public void NotifyAccountsAboutTransaction(String transactionCode)
        {
            SingletonMediator.Instance.NotifyColleagues<String>("OperationAdded", transactionCode);
        }

        public void EndMakeTransferToExternal(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    string transactionCode = OperationService.EndMakeTransferToExternal(result);
                    NotifyAccountsAboutTransaction(transactionCode);
                    ShowTransferOK(transactionCode);
                }
                catch (FaultException ex)
                {
                    Message = ex.Message;
                }
                catch (SecurityAccessDeniedException)
                {
                    ShowSecurityError();
                }
                finally
                {
                    InProgress = false;
                }
            }
        }
        #endregion

        private AccountViewModel _from;
        public AccountViewModel From
        {
            get { return _from; }
            set
            {
                _from = value;
                OnPropertyChanged(() => From);
                MakeTransferCommand.RaiseCanExecuteChanged();
            }
        }

        private AccountViewModel _to;
        public AccountViewModel To
        {
            get { return _to; }
            set
            {
                _to = value;
                OnPropertyChanged(() => To);
                MakeTransferCommand.RaiseCanExecuteChanged();
                ToExternal = _to == null;
            }
        }

        private bool _toExternal;
        public bool ToExternal
        {
            get { return _toExternal; }
            set { _toExternal = value; }
        }

        private String _message;

        public String Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(() => Message);
            }
        }

        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }

        private String _creditAccountIban;
        public String CreditAccountIban
        {
            get
            {
                return _creditAccountIban;
            }
            set
            {
                _creditAccountIban = value;
                OnPropertyChanged(() => CreditAccountIban);
                MakeTransferCommand.RaiseCanExecuteChanged();
            }
        }

        private String _motif;

        public String Motif
        {
            get { return _motif; }
            set { 
                _motif = value;
                OnPropertyChanged(() => Motif);
                MakeTransferCommand.RaiseCanExecuteChanged();
            }
        }

        private Decimal _amount;

        public Decimal Amount
        {
            get { return _amount; }
            set { 
                _amount = value;
                OnPropertyChanged(() => Amount);
                MakeTransferCommand.RaiseCanExecuteChanged();
            }
        }

        private PaymentEventViewModel _payment;

        public PaymentEventViewModel Payment
        {
          get { return _payment; }
          set { 
              _payment = value;
              CreditAccountIban = _payment.CreditAccountIban;
              
              //if the partner is not null - put the partner name inside the motif
              if (_payment.Partner != null)
              {
                  PartnerName = _payment.Partner.Title;
                  ToExternal = true;
                  Motif = _payment.Partner.Title;
              }

              Motif+= _payment.Description;
              Amount = _payment.Amount;
              From = _payment.Account;
              OnPropertyChanged(() => Payment);
          }
        }

        public void EndGetPaymentById(IAsyncResult result)
        {
            var paymet = PaymentEventService.EndGetPaymentEventById(result);
            if (paymet != null)
            {
                Motif = paymet.Description;
                CreditAccountIban = paymet.PartnerIban;
                Amount = paymet.Amount;
            }
        }

        private String _partnerName;

        public String PartnerName
        {
            get { return _partnerName; }
            set { 
                _partnerName = value;
                OnPropertyChanged(() => PartnerName);
            }
        }
    }
}
