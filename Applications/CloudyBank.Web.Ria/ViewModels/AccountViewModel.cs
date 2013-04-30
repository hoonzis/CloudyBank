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
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;
using CloudyBank.MVVM;
using CloudyBank.Web.Ria.Technical.XamlSerialization;
using CloudyBank.PortableServices.Accounts;
using CloudyBank.PortableServices.Operations;
using CloudyBank.PortableServices.TagDepenses;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        #region Ctors
        private bool _loadingOperations;

        public AccountViewModel()
        {
            SingletonMediator.Instance.Register(this);
        }

        public AccountViewModel(AccountDto account) : this()
        {
            Account = account;
        }
        #endregion

        #region Services
        
        private WCFAccountService _accountService;

        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFAccountService AccountService 
        {
            get
            {
                if (_accountService == null)
                {
                    _accountService = ServicesFactory.GetObject<WCFAccountService>();//, WCFAccountServiceClient>();  
                }
                return _accountService;
            }
        }

        private WCFOperationService _operationsService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFOperationService OperationsService
        {
            get
            {
                if (_operationsService == null)
                {
                    _operationsService = ServicesFactory.GetObject<WCFOperationService>();//, WCFOperationServiceClient>();
                }
                return _operationsService;
            }
            set
            {
                _operationsService = value;
            }
        }

        #endregion

        #region Account
        private AccountDto _account;
        public AccountDto Account
        {
            get { return _account; }
            set {
                _account = value;
                OnPropertyChanged(() => Account);
            }
        }

        public int Id
        {
            get { return _account.Id; }
        }

        public String Title
        {
            get { return _account.Title; }
            set
            {
                _account.Title = value;
                OnPropertyChanged(() => Title);
            }
        }

        public Decimal Balance
        {
            get { return _account.Balance; }
            set { 
                _account.Balance = value;
                OnPropertyChanged(()=>Balance);
                OnPropertyChanged(() => IsPositive);
                SingletonMediator.Instance.NotifyColleagues<Object>("MaxBalanceChanged",null);
            }
        }

        public String Number
        {
            get
            {
                return _account.Number;                
            }
            set
            {
                _account.Number = value;
                OnPropertyChanged(() => Number);
            }
        }

        public String Currency
        {
            get
            {
                return Account.Currency;
            }
        }

        public String Iban
        {
            get { return _account.Iban; }
        }

        public bool IsPositive
        {
            get { return Balance >= 0; }
            
        }

        #endregion

        #region Operations
        private ObservableCollection<OperationViewModel> _operations;
        public ObservableCollection<OperationViewModel> Operations
        {
            get {
                if (_operations == null)
                {
                    LoadOperations();
                }
                return _operations; 
            }
            set
            {
                _operations = value;
                
                OnPropertyChanged(() => Operations);
                OnPropertyChanged(() => LastMonthDepenses);
                OnPropertyChanged(() => LastMonthIncome);
                OnPropertyChanged(() => TagChartData);
                OnPropertyChanged(() => BalanceEvolution);
                OnPropertyChanged(() => FilteredBalanceEvolution);
                OnPropertyChanged(() => FilteredOperations);
                UpdateFilteredOperations();
                UpdateFilteredBalance();
                UpdateTagChartData();
            }
        }

        private ObservableCollection<OperationViewModel> _filteredOperations;
        public ObservableCollection<OperationViewModel> FilteredOperations
        {
            get { return _filteredOperations; }
            set
            {
                _filteredOperations = value;
                OnPropertyChanged(() => FilteredOperations);
            }
        }

        public void LoadOperations()
        {
            if (Account != null && !_loadingOperations)
            {
                InProgress = true;
                _loadingOperations = true;
                OperationsService.BeginGetOperationsByAccount(Account.Id, EndLoadOperations, null);   
            }
        }

        public void EndLoadOperations(IAsyncResult result)
        {
            var operations = OperationsService.EndGetOperationsByAccount(result);
            Operations = new ObservableCollection<OperationViewModel>(operations.Select(x => new OperationViewModel(x)));
            _loadingOperations = false;
            InProgress = false;
        }

        private CommandBase _updateAccountCommand;

        //command for updating opeations & balances - tags are computed dynamically on client side
        public CommandBase UpdateAccountCommand
        {
            get {
                if (_updateAccountCommand == null)
                {
                    _updateAccountCommand = new CommandBase(()=> {
                        LoadOperations();
                        LoadBalanceEvolution();
                    },() => true);
                }
                return _updateAccountCommand; 
            }
            set { _updateAccountCommand = value; }
        }
        #endregion

        #region Tag Chart Data

        //TODO: The tag depenese dictionary is of type Double because the AM.charts component has a bug does not support Decimal
        //also conversions is needed when building the dictionary
        private Dictionary<String, double> _tagChartData;

        public Dictionary<String, double> TagChartData
        {
            get {

                if (_tagChartData == null && AccountTimeSpan == 0)
                {
                    AccountTimeSpan = 30;
                }
                
                return _tagChartData; 
            }
            set { 
                _tagChartData = value;
                OnPropertyChanged(() => TagChartData);
            }
        }

        public void UpdateTagChartData()
        {
            if (Operations != null)
            {
               TagChartData = VMUtils.GetTagsRepartition(Operations, _compareDate);// data;
            }
        }

        #endregion

        #region Get & Add Operation
        public void AddOperation(int id)
        {
            OperationsService.BeginGetOperationById(id, EndGetOperationById, null);
        }

        public void GetOperationById(int operationID)
        {
            OperationsService.BeginGetOperationById(operationID, EndGetOperationById,null);
            InProgress = true;
        }

        public void EndGetOperationById(IAsyncResult result)
        {
            OperationDto operation = OperationsService.EndGetOperationById(result);
            OperationViewModel operationVM = Operations.SingleOrDefault(o => o.Operation.Id == operation.Id);
            if (operationVM != null)
            {
                operationVM.Operation = operation;
            }
            else
            {
                Operations.Add(new OperationViewModel(operation));
            }
            InProgress = false;
        }
        #endregion

        #region Balance Evolution
        private ObservableCollection<BalancePointViewModel> _balanceEvolution;

        public ObservableCollection<BalancePointViewModel> BalanceEvolution
        {
            get {
                if (_balanceEvolution == null)
                {
                    LoadBalanceEvolution();   
                }
                return _balanceEvolution;
            }
            set
            {
                _balanceEvolution = value;
                OnPropertyChanged(() => BalanceEvolution);
                OnPropertyChanged(() => LastMonthStart);
                OnPropertyChanged(() => FilteredBalanceEvolution);
            }
        }

        private ObservableCollection<BalancePointViewModel> _filteredBalanceEvolution;

        public ObservableCollection<BalancePointViewModel> FilteredBalanceEvolution
        {
            get {
                if (_filteredBalanceEvolution == null)
                {
                    UpdateFilteredBalance();
                }
                return _filteredBalanceEvolution; 
            }
            set
            {
                _filteredBalanceEvolution = value;
                OnPropertyChanged(() => FilteredBalanceEvolution);
            }
        }

        private void LoadBalanceEvolution()
        {
            //if account is null - the id is not defined
            if (Account == null)
            {
                return;
            }

            AccountService.BeginGetAccountEvolution(Id, EndGetAccountEvolution, null);
        }

        private void EndGetAccountEvolution(IAsyncResult result)
        {
            BalanceEvolution = new ObservableCollection<BalancePointViewModel>(AccountService.EndGetAccountEvolution(result).Select(x=>new BalancePointViewModel(x)));
        }

        private Dictionary<DateTime, BalancePointViewModel> _balanceDictionary;

        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public Dictionary<DateTime, BalancePointViewModel> BalanceDictionary
        {
            get {
                if (_balanceDictionary == null)
                {
                    _balanceDictionary = BalanceEvolution.GroupBy(x => x.Date).ToDictionary(v => v.Key, v=>v.First());
                }
                return _balanceDictionary; 
            }
            set { _balanceDictionary = value; }
        }
        

        public decimal LastMonthStart
        {
            get
            {
                if (BalanceEvolution != null && BalanceEvolution.Count > 0)
                {
                    
                    //get the date of last month
                    var lastMonth = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
                    
                    
                    if (BalanceDictionary.Count>0)
                    {
                        return BalanceDictionary[lastMonth.Date].Balance;
                    }

                    #region Version for the situation where not all date times are in the dictionary
                    //else
                    //{
                    //    while (!BalanceDictionary.ContainsKey(lastMonth.Date))
                    //    {
                    //        lastMonth = lastMonth.Subtract(new TimeSpan(1, 0, 0, 0));
                    //    }
                    //    return BalanceDictionary[lastMonth.Date].Balance;
                    //}
                    #endregion
                }
                return 0;
            }
        }

        public decimal LastMonthDepenses
        {
            get
            {
                if (Operations != null && Operations.Count > 0)
                {
                    var lastMonth = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
                    return Operations.Where(x => x.Date.CompareTo(lastMonth) > 0)
                        .Where(x => x.Amount < 0)
                        .Sum(x => x.Amount);
                }
                return 0;
            }
        }

        public decimal LastMonthIncome
        {
            get
            {
                if (Operations != null && Operations.Count > 0)
                {
                    var lastMonth = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0));
                    return Operations.Where(x => x.Date.CompareTo(lastMonth) > 0)
                        .Where(x => x.Amount > 0)
                        .Sum(x => x.Amount);
                }
                return 0;
            }
        }
        #endregion

        #region TimeSpan & Compare Date
        private DateTime _compareDate;

        public DateTime CompareDate
        {
            get
            {
                if (AccountTimeSpan == 0)
                {
                    AccountTimeSpan = 30;
                }
                return _compareDate;
            }

            set
            {
                if (_compareDate != value)
                {
                    _compareDate = value;
                    OnPropertyChanged(() => CompareDate);

                    UpdateTagChartData();

                    UpdateFilteredBalance();
                    
                    UpdateFilteredOperations();

                }
            }
        }

        public void UpdateFilteredOperations()
        {
            if (Operations == null)
            {
                return;
            }

            FilteredOperations = new ObservableCollection<OperationViewModel>(Operations.Where(x => x.Date.CompareTo(_compareDate) > 0));
        }

        public void UpdateFilteredBalance()
        {
            if (BalanceEvolution == null)
            {
                return;
            }

            FilteredBalanceEvolution = new ObservableCollection<BalancePointViewModel>(BalanceEvolution.Where(x => x.Date.CompareTo(CompareDate) > 0));
            
            //This was and intent to perform some optimization.
            //it was done by removing the unecessary points, which finally does not speedup and makes it more complicated
            #region optim
            //ObservableCollection<BalancePoint> filtered = new ObservableCollection<BalancePoint>();
            //BalancePoint previous = null;

            //foreach (var point in BalanceEvolution)
            //{
            //    if (point.Date.CompareTo(_compareDate) > 0)
            //    {
            //        if (previous == null)
            //        {
            //            filtered.Add(point);
            //        }

            //        if (previous != null && previous.Balance != point.Balance)
            //        {
            //            if (filtered.Select(x => x.Date == previous.Date).Count() == 0)
            //            {
            //                filtered.Add(previous);
            //            }
            //            filtered.Add(point);
            //        }
            //        previous = point;
            //    }
            //}
            //FilteredBalanceEvolution = filtered;
            #endregion
        }

        private int _accountTimeSpan;

        public int AccountTimeSpan
        {
            get {
                return _accountTimeSpan;
            }
            set {
                if (_accountTimeSpan != value)
                {
                    _accountTimeSpan = value;
                    OnPropertyChanged(() => _accountTimeSpan);

                    if (_accountTimeSpan != TimeSpan.MaxValue.Days)
                    {
                        CompareDate = DateTime.Now.Subtract(new TimeSpan(_accountTimeSpan, 0, 0, 0));
                    }
                    else
                    {
                        CompareDate = DateTime.MinValue;
                    }
                }
            }
        }

        private bool _inProgress;

        public bool InProgress
        {
            get { return _inProgress; }
            set {
 
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }


        #endregion

        #region Listening Methods for communication between viewmodels
        [MediatorMessageSink("OperationAdded", ParameterType = typeof(String))]
        public void OnOperationAdded(String code)
        {
            //update all operations
            LoadOperations();
        }

        
        [MediatorMessageSink("OperationTagged", ParameterType = typeof(int))]
        public void OnOperationTagged(int id)
        {
            if (Operations == null)
            {
                return;
            }
            
            var operation = Operations.FirstOrDefault(x=>x.Operation.Id == id);
            
            
            if(operation!=null){
                operation.Update();
                //Possible improvement - just change the data in the ObservableCollection of TagDepenses.
                UpdateTagChartData();
            }
        }
        #endregion
    }
}
