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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CloudyBank.Web.Ria.Technical.XamlSerialization;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using CloudyBank.PortableServices.Accounts;
using CloudyBank.PortableServices.Customers;
using CloudyBank.PortableServices.Tags;
using CloudyBank.PortableServices.PaymentEvents;
using CloudyBank.PortableServices.TagDepenses;
using CloudyBank.PortableServices.Partners;
using CloudyBank.PortableServices.OAuthTokens;


namespace CloudyBank.Web.Ria.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        public const String EMAIL_REGEX = "^(([A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(\\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)|(\"([ !#-\\[\\]-~]|\\\\[ -~])*\"))@((([a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9])(\\.[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9])*)|(\\[[0-9]{1,3}(\\.[0-9]{1,3}){3}\\])|(\\[IPv6:[0-9a-fA-F:]+(:\\[[0-9]{1,3}(\\.[0-9]{1,3}){3}\\])?\\]))$";

        private bool _partnersLoading;
        #region Ctors
       
        //always leave one empty constructor for the unit tests
        public CustomerViewModel()
        {
            SingletonMediator.Instance.Register(this);
        }

        public CustomerViewModel(int id) : this()
        {
            GetCustomerByID(id);
        }

        public CustomerViewModel(CustomerDto dto) : this()
        {
            Customer = dto;
        }

        #endregion

        #region Services
        private WCFCustomersService _customerService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFCustomersService CustomerService
        {
            get {
                if (_customerService == null)
                {
                    _customerService = ServicesFactory.GetObject<PortableServices.Customers.WCFCustomersService>();//, WCFCustomersServiceClient>();// new WCFCustomersServiceClient();
                }
                return _customerService;
            }
            set
            {
                _customerService = value;
            }
        }

        private WCFAccountService _accountService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFAccountService AccountService
        {
            get
            {
                if (_accountService == null)
                {
                    _accountService = ServicesFactory.GetObject<WCFAccountService>();//, WCFAccountServiceClient>();// AccountService;// new WCFAccountServiceClient();
                }
                return _accountService;
            }
            set { _accountService = value; }
        }

        private WCFTagService _tagService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        private WCFTagService TagService
        {
            get {
                if (_tagService == null)
                {
                    _tagService = ServicesFactory.GetObject<WCFTagService>();//, WCFTagServiceClient>();// new WCFTagServiceClient();
                }
                return _tagService;
            }
        }

        public WCFPartnerService _partnerService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFPartnerService PartnerService
        {
            get {
                if (_partnerService == null)
                {
                    _partnerService = ServicesFactory.GetObject<WCFPartnerService>();//, WCFPartnerServiceClient>();// PartnerService;// new WCFPartnerServiceClient();
                }
                return _partnerService;
            }
            set
            {
                _partnerService = value;
            }
        }

        private WCFPaymentEventService  _paymentEventService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFPaymentEventService PaymentEventService
        {
            get
            {
                if (_paymentEventService == null)
                {
                    _paymentEventService = ServicesFactory.GetObject<WCFPaymentEventService>();//, WCFPaymentEventServiceClient>(); //PaymentEventService;// new WCFPaymentEventServiceClient();
                }
                return _paymentEventService;
            }

        }

        private WCFTagDepensesService _tagDepensesService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFTagDepensesService TagDepensesService
        {
          get {
              if (_tagDepensesService == null)
              {
                  _tagDepensesService = ServicesFactory.GetObject<WCFTagDepensesService>();//, WCFTagDepensesServiceClient>(); // new WCFTagDepensesServiceClient();
              }
              return _tagDepensesService; }
          set { _tagDepensesService = value; }
        }

        private WCFOAuthManagementService _oAuthService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFOAuthManagementService OAuthService
        {
            get
            {
                if (_oAuthService == null)
                {
                    _oAuthService = ServicesFactory.GetObject<WCFOAuthManagementService>();//, WCFOAuthManagementServiceClient>();
                }
                return _oAuthService;
            }
        }

        #endregion
        
        #region Customer

        //only customers which filled the situation status can view the tags repartition of other clients
        public bool HasProfile
        {
            get
            {
                return FamilySituation != FamilySituation.NotSet;
            }
        }
        
        public IAsyncResult GetCustomerByID(int customerID)
        {
            InProgress = true;
            return CustomerService.BeginGetCustomerByID(customerID, EndGetCustomerByID, null);
        }

        public void EndGetCustomerByID(IAsyncResult result)
        {
            Customer = CustomerService.EndGetCustomerByID(result);
            InProgress = false;
        }

        private CustomerDto _customer;
        public CustomerDto Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(() => Customer);
                OnPropertyChanged(() => Id);
                OnPropertyChanged(() => Name);
                OnPropertyChanged(() => FirstName);
                OnPropertyChanged(() => LastName);
                OnPropertyChanged(() => Email);
                OnPropertyChanged(() => Accounts);
                OnPropertyChanged(() => Tags);
                OnPropertyChanged(() => Partners);
                OnPropertyChanged(() => TagDepenses);
                OnPropertyChanged(() => PhoneNumber);
                OnPropertyChanged(() => Tokens);
            }
        }

        public String Email 
        {
            get {
                if (_customer != null)
                {
                    return _customer.Email;
                }
                return ViewModelsResources.Loading;
            }
            set {
                RemoveErrors("Email");
                CheckEmail(value);
                _customer.Email = value;
                OnPropertyChanged(() => Email);
            }
        }

        private void CheckEmail(String email)
        {
            if (String.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email,EMAIL_REGEX))
            {
                AddError("Email", ViewModelsResources.IncorrectEmail);
            }
        }
        public String Code
        {
            get
            {
                return Customer.Code;
            }
        }

        public int Id
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.Id;
                }
                return 0;
            }
        }

        public String FirstName
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.FirstName;
                }
                return ViewModelsResources.Loading;
            }
        }

        public String LastName
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.LastName;
                }
                return ViewModelsResources.Loading;
            }
        }

        public String Name
        {
            get
            {
                if (Customer != null)
                {
                    return String.Format("{0} {1}", FirstName, LastName);
                }
                return ViewModelsResources.Loading;
            }
        }

        public String PhoneNumber
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.PhoneNumber;
                }
                return ViewModelsResources.Loading;
            }
            set
            {
                Customer.PhoneNumber = value;
                OnPropertyChanged(() => PhoneNumber);
            }

        }

        public FamilySituation FamilySituation
        {
            get
            {
                if (Customer != null)
                {
                    return Customer.Situation;
                }
                return FamilySituation.NotSet;
            }
            set
            {
                Customer.Situation = value;
            }
        }

        #endregion

        #region Accounts
        private ObservableCollection<AccountViewModel> _accounts;
        public ObservableCollection<AccountViewModel> Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    LoadAccounts();
                    //Get tags at the same time - to speed up
                    LoadTags();
                }
                return _accounts;
            }
            set
            {
                if (_accounts != value)
                {
                    _accounts = value;

                    //if there are some accounts - select the first one
                    if (_accounts != null && _accounts.Count > 0)
                    {
                        SelectedAccount = _accounts[0];
                    }

                    OnPropertyChanged(() => Accounts);
                    OnPropertyChanged(() => Sum);
                }
            }
        }

        private ObservableCollection<AccountViewModel> _selectedAccounts;

        public ObservableCollection<AccountViewModel> SelectedAccounts
        {
            get {
                if (_selectedAccounts == null)
                {
                    _selectedAccounts = new ObservableCollection<AccountViewModel>();
                }
                return _selectedAccounts; 
            }
            set
            {
                _selectedAccounts = value;
                OnPropertyChanged(() => SelectedAccounts);
            }
        }

        public void LoadAccounts()
        {
            if (Customer != null)
            {
                AccountService.BeginGetAccountsByCustomer(Customer.Id, EndLoadAccounts, null);
                InProgress = true;
            }
        }

        public void EndLoadAccounts(IAsyncResult e)
        {
            var accounts = AccountService.EndGetAccountsByCustomer(e).Select(x => new AccountViewModel(x));
            Accounts = new ObservableCollection<AccountViewModel>(accounts);
            InProgress = false;
            OnPropertyChanged(() => Sum);
        }

        public int NumberOfAccounts
        {
            get { return Accounts.Count; }
        }

        public decimal MaxBalance
        {
            get { return Accounts.Max(x => Math.Abs(x.Balance)); }
        }

        public decimal Sum
        {
            get
            {
                if (Accounts != null)
                {
                    return Accounts.Sum(x => x.Balance);
                }
                return 0;
            }
        }

        private AccountViewModel _selectedAccount;
        public AccountViewModel SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(() => SelectedAccount);
            }
        }

        #endregion
        
        #region Check & Uncheck accounts
        private CommandBaseGeneric<AccountViewModel> _accountCheckedCommand;
        public CommandBaseGeneric<AccountViewModel> AccountCheckedCommand
        {
            get
            {
                if (_accountCheckedCommand == null)
                {
                    _accountCheckedCommand = new CommandBaseGeneric<AccountViewModel>(AccountChecked, null);
                }
                return _accountCheckedCommand;
            }
        }

        public void AccountChecked(AccountViewModel account)
        {
            SelectedAccounts.Add(account);
            OnPropertyChanged(() => FilteredEvents);
            OnPropertyChanged(() => ThisMonthEvents);
        }

        private CommandBaseGeneric<AccountViewModel> _accountUncheckedCommand;

        public CommandBaseGeneric<AccountViewModel> AccountUncheckedCommand
        {
            get
            {
                if (_accountUncheckedCommand == null)
                {
                    _accountUncheckedCommand = new CommandBaseGeneric<AccountViewModel>(AccountUnchecked, null);
                }
                return _accountUncheckedCommand;
            }
        }

        public void AccountUnchecked(AccountViewModel account)
        {
            SelectedAccounts.Remove(account);
            OnPropertyChanged(() => FilteredEvents);
            OnPropertyChanged(() => ThisMonthEvents);
        }
        #endregion

        #region SaveCommand
        private CommandBase _saveCommand;

        public CommandBase SaveCommand
        {
            get
            {
                if(_saveCommand == null){
                    _saveCommand = new CommandBase(()=>Save(),()=>CanSave());
                }
                return _saveCommand;
            }
        }

        public void Save()
        {
            CustomerService.BeginSaveCustomerDto(Customer, EndSave, null);
        }

        public void EndSave(IAsyncResult e)
        {
            if (e.IsCompleted)
            {
                LoadAccounts();
            }
        }

        public bool CanSave()
        {
            return !HasErrors;
        }

        #endregion

        #region Tags

        
        private ObservableCollection<TagViewModel> _tags;
        public ObservableCollection<TagViewModel> Tags
        {
            get {
                if (_tags == null)
                {
                    LoadTags();
                }
                return _tags; 
            }
            set { 
                _tags = value;
                OnPropertyChanged(() => Tags);
            }
        }

        public void LoadTags()
        {
            if (Customer != null)
            {
                TagService.BeginGetTagsForCustomer(Customer.Id, EndLoadTags, null);
            }
        }

        public void EndLoadTags(IAsyncResult result)
        {
            var tags = TagService.EndGetTagsForCustomer(result).Select(x=>new TagViewModel(x));
            Tags = new ObservableCollection<TagViewModel>(tags);
        }

        
        #endregion
        
        #region Partners

        private ObservableCollection<BusinessPartnerViewModel> _partners;

        public ObservableCollection<BusinessPartnerViewModel> Partners
        {
            get {
                if (_partners == null)
                {
                    LoadPartners();
                }
                return _partners;
            }
            set { 
                _partners = value;
                OnPropertyChanged(() => Partners);
            }
        }

        public IAsyncResult LoadPartners()
        {
            
            if (Customer != null && !_partnersLoading)
            {
                InProgress = true;
                _partnersLoading = true;
                return PartnerService.BeginGetPartnersForCustomer(Customer.Id, EndGetPartners, null);
            }
            return null;
        }

        public void EndGetPartners(IAsyncResult result)
        {
            var partners = new ObservableCollection<BusinessPartnerViewModel>(PartnerService.EndGetPartnersForCustomer(result).Select(x => new BusinessPartnerViewModel(x)));
            Partners = partners;
            InProgress = false;
            _partnersLoading = false;
        }

        #endregion

        #region TagDepenses
        private Dictionary<String, double> _tagDepenses;
        public Dictionary<String, double> TagDepenses
        {      
    
            get
            {
                return _tagDepenses;
            }
            set
            {
                _tagDepenses = value;
                OnPropertyChanged(() => TagDepenses);
            }
        }

        private int _tagDepensesDaysCount;
        public int TagDepensesDaysCount
        {
            get
            {
                return _tagDepensesDaysCount;
            }
            set
            {
                
                if(_tagDepensesDaysCount!=value){
                    _tagDepensesDaysCount = value;
                    OnPropertyChanged(() => TagDepensesDaysCount);

                    DateTime compareDate;
                    if (TagDepensesDaysCount == TimeSpan.MaxValue.Days)
                    {
                        compareDate = DateTime.MinValue;
                    }
                    else
                    {
                        compareDate = DateTime.Now.Subtract(new TimeSpan(TagDepensesDaysCount, 0, 0, 0));
                    }

                    UpdateTagDepenses(compareDate);
                }
            }
        }

        private void UpdateTagDepenses(DateTime compareDate)
        {
            if (Accounts == null)
            {
                return;
            }

            var operations = Accounts.Where(x=>x.Operations!=null).SelectMany(x => x.Operations);
            TagDepenses = VMUtils.GetTagsRepartition(operations, compareDate);

            
            //var data = operations
            //    .Where(x=>!String.IsNullOrWhiteSpace(x.TagName))
            //    .Where(x => x.Amount < 0)
            //    .Where(x => x.Date.CompareTo(compareDate) > 0)
            //    .GroupBy(x => x.TagName)
            //    .ToDictionary(x => x.Key, x => Math.Abs(x.Sum(y => y.Amount)));

            //TagDepenses = data;
        }

        #endregion

        #region PaymentEvents

        ObservableCollection<PaymentEventViewModel> _paymentEvents;

        public ObservableCollection<PaymentEventViewModel> PaymentEvents
        {
            get
            {
                if (_paymentEvents == null)
                {
                    LoadPaymentEvents();
                }
                return _paymentEvents;
            }
            set
            {
                _paymentEvents = value;
                
                OnPropertyChanged(() => PaymentEvents);
                OnPropertyChanged(() => FilteredEvents);
                OnPropertyChanged(() => ThisMonthEvents);
            }
        }
        
        public void LoadPaymentEvents()
        {
            if (Customer != null)
            {
                PaymentEventService.BeginGetPaymentEventsForCustomer(Customer.Id, EndLoadPaymentEvents, null);
            }
        }

        public void EndLoadPaymentEvents(IAsyncResult result)
        {
            //in the case when partners and accounts are already loaded - load the payments and get account and partner for each payment by id
            if (Partners != null && Accounts != null)
            {
                PaymentEvents = new ObservableCollection<PaymentEventViewModel>(PaymentEventService.EndGetPaymentEventsForCustomer(result).Select(
                    x => new PaymentEventViewModel(x)
                    {
                        Account = Accounts.FirstOrDefault(y => y.Account.Id == x.AccountId),
                        Partner = Partners.FirstOrDefault(y => y.Id == x.PartnerId)
                    }));
            }
        }

        //events only for selected accounts
        public ObservableCollection<PaymentEventViewModel> FilteredEvents
        {
            get
            {
                if (PaymentEvents == null)
                {
                    return null;
                }

                var payments = from payment in PaymentEvents where SelectedAccounts.Contains(payment.Account) select payment;
                if (payments != null)
                {
                    return new ObservableCollection<PaymentEventViewModel>(payments.OrderBy(x => x.Date));
                }
                return null;
            }
        }

        //events of this month only for selected accounts
        public ObservableCollection<PaymentEventViewModel> ThisMonthEvents
        {
            get
            {
                if (FilteredEvents == null)
                {
                    return null;
                }

                var payments = FilteredEvents.Where(x => x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year);

                if (payments != null)
                {
                    return new ObservableCollection<PaymentEventViewModel>(payments.OrderBy(x => x.Date));
                }
                return null;
            }
        }

        private CommandBaseGeneric<PaymentEventViewModel> _updatePaymentCommand;

        public CommandBaseGeneric<PaymentEventViewModel> UpdatePaymentCommand
        {
            get
            {
                if (_updatePaymentCommand == null)
                {
                    _updatePaymentCommand = new CommandBaseGeneric<PaymentEventViewModel>(UpdateOrSavePayment, CanUpdatePayment);
                }
                return _updatePaymentCommand;
            }

        }

        public void UpdateOrSavePayment(PaymentEventViewModel vm)
        {
            if (vm.IsNew)
            {
                PaymentEventService.BeginCreatePaymentEvent(vm.Payment, Customer.Id, EndCreatePaymentEvent, vm);
                InProgress = true;
            }
            else
            {
                PaymentEventService.BeginUpdatePaymentEvent(vm.Payment, EndUpdatePaymentEvent, vm);
                InProgress = true;
            }
        }

        public void EndCreatePaymentEvent(IAsyncResult result)
        {
            try
            {
                int id = PaymentEventService.EndCreatePaymentEvent(result);
                if (id != -1)
                {
                    //get the freshly add payment
                    PaymentEventViewModel paymentEvent = result.AsyncState as PaymentEventViewModel;
                    paymentEvent.Id = id;
                    paymentEvent.IsNew = false;
                    //add it to the collection
                    PaymentEvents.Add(paymentEvent);

                    //force the update of the 
                    OnPropertyChanged(() => FilteredEvents);
                    OnPropertyChanged(() => ThisMonthEvents);
                }
            }
            catch (Exception)
            {
                ErrorMessage = ViewModelsResources.ErrorSavingPaymentEvent;
                IsError = true;
            }
            InProgress = false;
            
        }

        public void EndUpdatePaymentEvent(IAsyncResult result)
        {
            

            if (result.IsCompleted)
            {
                try
                {
                    bool success = PaymentEventService.EndUpdatePaymentEvent(result);
                    if (success)
                    {
                        OnPropertyChanged(() => FilteredEvents);
                    }
                    else
                    {
                        ErrorMessage = ViewModelsResources.ErrorSavingPayment;
                        IsError = true;
                    }
                }
                catch (FaultException ex)
                {
                    ErrorMessage = ex.Message;
                    IsError = true;
                }
            }
            InProgress = false;
        }

        public void EndGetPaymentById(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    PaymentEventDto paymentEvent = PaymentEventService.EndGetPaymentEventById(result);
                    PaymentEvents.Add(new PaymentEventViewModel(paymentEvent));
                }
                catch (FaultException ex)
                {
                    IsError = true;
                    ErrorMessage = ex.Message;
                }
            }
            InProgress = false;
        }

        public bool CanUpdatePayment(PaymentEventViewModel vm)
        {
            if (vm != null)
            {
                vm.RunWhenErrorsChange = UpdatePartnerCommand.RaiseCanExecuteChanged;
            }
            return vm != null && !vm.HasErrors;
        }

        private CommandBaseGeneric<PaymentEventViewModel> _removePaymentCommand;

        public CommandBaseGeneric<PaymentEventViewModel> RemovePaymentCommand
        {
            get
            {
                if (_removePaymentCommand == null)
                {
                    _removePaymentCommand = new CommandBaseGeneric<PaymentEventViewModel>(RemovePaymentEvent, CanRemovePaymentEvent);
                }
                return _removePaymentCommand;

            }

        }

        public void RemovePaymentEvent(PaymentEventViewModel vm)
        {
            if (!vm.IsNew)
            {
                PaymentEventService.BeginRemovePaymentEvent(vm.Id, EndRemovePaymentEvent, vm);
                InProgress = true;
            }
        }

        public bool CanRemovePaymentEvent(PaymentEventViewModel vm)
        {
            return vm!=null && !vm.IsNew;
        }

        public void EndRemovePaymentEvent(IAsyncResult result)
        {
           
            if (result.IsCompleted)
            {
                try
                {
                    bool success = PaymentEventService.EndRemovePaymentEvent(result);
                    if (success)
                    {
                        PaymentEvents.Remove(result.AsyncState as PaymentEventViewModel);
                    }
                    OnPropertyChanged(() => FilteredEvents);
                }
                catch (FaultException ex)
                {
                    IsError = true;
                    ErrorMessage = ex.Message;
                }
            }
            InProgress = false;
        }

        #endregion

        #region PartnerCommands

        private CommandBaseGeneric<BusinessPartnerViewModel> _updatePartnerCommand;

        public CommandBaseGeneric<BusinessPartnerViewModel> UpdatePartnerCommand
        {
            get
            {
                if (_updatePartnerCommand == null)
                {
                    _updatePartnerCommand = new CommandBaseGeneric<BusinessPartnerViewModel>((partner) => { UpdatePartner(partner);} , CanUpdatePartner);
                }
                return _updatePartnerCommand;
            }
        }

        public IAsyncResult UpdatePartner(BusinessPartnerViewModel vm)
        {
            if (Customer == null)
            {
                return null;
            }

            InProgress = true;
            if (vm.IsNew)
            {
                return PartnerService.BeginCreatePartner(vm.Partner, Customer.Id, EndCreatePartner, vm);
            }
            else
            {
                return PartnerService.BeginUpdatePartner(vm.Partner,Customer.Id, EndUpdatePartner, vm);
            }
            
        }

        public void EndCreatePartner(IAsyncResult result)
        {
            try
            {
                int id = PartnerService.EndCreatePartner(result);
                if (id != -1)
                {
                    BusinessPartnerViewModel partner = result.AsyncState as BusinessPartnerViewModel;
                    partner.Id = id;
                    partner.IsNew = false;
                }
                else
                {
                    ErrorMessage = ViewModelsResources.ErrorCreatingBP;
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                IsError = true;
            }
            
            InProgress = false;
        }

        public void EndUpdatePartner(IAsyncResult result)
        {
            InProgress = false;

            if (result.IsCompleted)
            {
                try
                {
                    bool success = PartnerService.EndUpdatePartner(result);
                    if (!success)
                    {
                        var partner = result.AsyncState as BusinessPartnerViewModel;
                        Partners.Remove(partner);

                        ErrorMessage = ViewModelsResources.ErrorSavingPartner;
                        IsError = true;
                    }
                }
                catch (FaultException)
                {
                    ErrorMessage = ViewModelsResources.ErrorSavingPartner;
                    IsError = true;
                }
            }
        }

        public bool CanUpdatePartner(BusinessPartnerViewModel vm)
        {
            //simply see if vm object does not have any errors
            return vm != null && !vm.HasErrors;
        }

        private CommandBaseGeneric<BusinessPartnerViewModel> _removePartnerCommand;

        public CommandBaseGeneric<BusinessPartnerViewModel> RemovePartnerCommand
        {
            get
            {
                if (_removePartnerCommand == null)
                {
                    _removePartnerCommand = new CommandBaseGeneric<BusinessPartnerViewModel>(RemovePartner, CanRemovePartner);
                }
                return _removePartnerCommand;
            }

        }

        public void RemovePartner(BusinessPartnerViewModel vm)
        {
            if (!vm.IsNew)
            {
                PartnerService.BeginRemovePartner(vm.Id, Customer.Id, EndRemovePartner, vm);
                InProgress = true;
            }
        }

        public bool CanRemovePartner(BusinessPartnerViewModel vm)
        {
            if (vm == null)
            {
                return false;
            }

            //new partner cannot be removed
            return !vm.IsNew;
        }

        public void EndRemovePartner(IAsyncResult result)
        {
            InProgress = false;
            if (result.IsCompleted)
            {
                try
                {
                    bool success = PartnerService.EndRemovePartner(result);
                    if (!success)
                    {
                        DelegateAction(()=> Partners.Add(result.AsyncState as BusinessPartnerViewModel));
                    }
                }
                catch (FaultException ex)
                {
                    IsError = true;
                    ErrorMessage = ex.Message;
                }
            }
        }

        #endregion

        #region Tag Commands
        #region CreateTag

        public void CreateTag(TagViewModel tag)
        {
            TagService.BeginCreateTag(Customer.Id, tag.Tag, EndCreateTag, null);
        }

        public void EndCreateTag(IAsyncResult result)
        {
            TagService.EndCreateTag(result);
        }

        private CommandBaseGeneric<TagViewModel> _createTagCommand;

        public CommandBaseGeneric<TagViewModel> CreateTagCommand
        {
            get
            {
                if (_createTagCommand == null)
                {
                    _createTagCommand = new CommandBaseGeneric<TagViewModel>(CreateTag, CanCreateTag);
                }
                return _createTagCommand;
            }
        }

        public bool CanCreateTag(TagViewModel tag)
        {
            return true;
        }

        #endregion

        #region UpdateTag

        private CommandBaseGeneric<TagViewModel> _updateTagCommand;

        public CommandBaseGeneric<TagViewModel> UpdateTagCommand
        {
            get
            {
                if (_updateTagCommand == null)
                {
                    _updateTagCommand = new CommandBaseGeneric<TagViewModel>(UpdateTag, CanUpdateTag);
                }
                return _updateTagCommand;
            }
        }

        public void UpdateTag(TagViewModel tag)
        {
            if (tag.IsNew)
            {
                TagService.BeginCreateTag(Customer.Id, tag.Tag, EndCreateTag, null);
            }
            else
            {
                TagService.BeginUpdateTag(tag.Tag, Customer.Id, EndUpdateTag, null);
            }
        }

        public void EndUpdateTag(IAsyncResult result)
        {
            TagService.EndUpdateTag(result);
        }

        public bool CanUpdateTag(TagViewModel tag)
        {
            return true;
        }

        #endregion

        #region RemoveTag

        CommandBaseGeneric<TagViewModel> _removeTagCommand;

        public CommandBaseGeneric<TagViewModel> RemoveTagCommand
        {
            get
            {
                if (_removeTagCommand == null)
                {
                    _removeTagCommand = new CommandBaseGeneric<TagViewModel>(RemoveTag, CanRemoveTag);
                }
                return _removeTagCommand;
            }
        }

        public void RemoveTag(TagViewModel tag)
        {
            TagService.BeginRemoveTag(tag.Id, Customer.Id, EndRemoveTag, null);
        }

        //New tag is not yet persisted.
        public bool CanRemoveTag(TagViewModel tag) { return !tag.IsNew; }

        public void EndRemoveTag(IAsyncResult result)
        {
            TagService.EndRemoveTag(result);
        }

        #endregion
        #endregion

        #region Messages & Indications
        private bool _inProgress = false;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged(() => IsError);
            }
        }

        private String _errorMessage;
        public String ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }
        #endregion

        #region Tokens
        private ObservableCollection<TokenViewModel> _tokens;

        public ObservableCollection<TokenViewModel> Tokens
        {
            get {
                if (_tokens == null)
                {
                    LoadTokens();
                }
                return _tokens; }
            set { 
                _tokens = value;
                OnPropertyChanged(() => Tokens);
            }
        }

        private void LoadTokens()
        {
            if (Customer != null)
            {
                OAuthService.BeginGetTokensForCustomer(Customer.Id, EndGetTokens, null);
            }
        }

        private void EndGetTokens(IAsyncResult result)
        {
            var tokens = OAuthService.EndGetTokensForCustomer(result);
            Tokens = new ObservableCollection<TokenViewModel>(tokens.Select(x=>new TokenViewModel(x)));
        }

        private CommandBase _loadTokensCommand;

        public CommandBase LoadTokensCommand
        {
            get {
                if (_loadTokensCommand == null)
                {
                    _loadTokensCommand = new CommandBase(LoadTokens, () => true);
                }
                return _loadTokensCommand; 
            }
            set { _loadTokensCommand = value; }
        }
        #endregion

        #region Listening methods - View Models communication

        [MediatorMessageSink("MaxBalanceChanged", ParameterType = typeof(Object))]
        public void MaxBalanceChanged(Object obj)
        {
            OnPropertyChanged(() => MaxBalance);
        }

        [MediatorMessageSink("RemovedToken",ParameterType= typeof(int))]
        public void OnTokenRemoved(int tokenID)
        {
            var tokenVM = Tokens.FirstOrDefault(x => x.Id == tokenID);
            DelegateAction(() => Tokens.Remove(tokenVM));
        }
        #endregion
    }
}
