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
using CloudyBank.MVVM;
using System.Linq;
using CloudyBank.PortableServices.Advisors;
using CloudyBank.PortableServices.Customers;
using CloudyBank.PortableServices.Accounts;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class AdvisorViewModel : ViewModelBase
    {
        public AdvisorViewModel()
        {

        }

        public AdvisorViewModel(int advisorID)
        {
            LoadCurrentAdvisor();
            LoadCustomersForAdvisor(advisorID);
        }

        #region Services
        private WCFCustomersService _customerService;
        public WCFCustomersService CustomerService
        {
            get
            {
                if (_customerService == null)
                {
                    _customerService = new WCFCustomersServiceClient();
                }
                return _customerService;
            }
        }

        private WCFAdvisorService _advisorService;

        public WCFAdvisorService AdvisorService
        {
            get {
                if (_advisorService == null)
                {
                    _advisorService = new WCFAdvisorServiceClient();
                }
                return _advisorService;
            }
            set { _advisorService = value; }
        }

        #endregion

        #region Advisor
        private AdvisorDto _advisor;

        public AdvisorDto Advisor
        {
            get { return _advisor; }
            set { 
                _advisor = value;
                OnPropertyChanged(() => Advisor);
            }
        }

        public void LoadCurrentAdvisor()
        {
            AdvisorService.BeginGetCurrentAdvisor(EndGetCurrentAdvisor, null);
        }

        public void EndGetCurrentAdvisor(IAsyncResult e)
        {
            Advisor = AdvisorService.EndGetCurrentAdvisor(e);
        }

        #endregion

        #region Customers
        ObservableCollection<CustomerViewModel> _customers;
        public ObservableCollection<CustomerViewModel> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged(() => Customers);
            }
        }

        public void LoadCustomersForAdvisor(int advisorId)
        {
            CustomerService.BeginGetCustomersForAdvisor(advisorId, EndGetCustomers, null);
        }

        public void EndGetCustomers(IAsyncResult e)
        {
            var customers = CustomerService.EndGetCustomersForAdvisor(e);
            if (customers != null)
            {
                Customers = new ObservableCollection<CustomerViewModel>(customers.Select(x => new CustomerViewModel(x)));
            }
        }
        #endregion

        ObservableCollection<CustomerViewModel> _individualCustomers;
        public ObservableCollection<CustomerViewModel> IndividualCustomers
        {
            get { return _individualCustomers; }
            set { 
                _individualCustomers = value;
                OnPropertyChanged(() => IndividualCustomers);
            }
        }

        private CustomerViewModel _currentCustomer;
        public CustomerViewModel CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                if (_currentCustomer != value)
                {
                    _currentCustomer = value;
                    OnPropertyChanged(() => CurrentCustomer);
                }
            }
        }

        private AccountDto _newAccount;

        public AccountDto NewAccount
        {
            get { return _newAccount; }
            set
            {
                _newAccount = value;
                OnPropertyChanged(() => NewAccount);
            }
        }
    }
}
