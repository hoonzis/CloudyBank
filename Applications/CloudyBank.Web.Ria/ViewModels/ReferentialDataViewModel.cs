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
using CloudyBank.MVVM;
using System.ServiceModel;
using System.ServiceModel.Security;
using CloudyBank.PortableServices.Agencies;
using CloudyBank.PortableServices.CustomerProfiles;
using CloudyBank.PortableServices.Tags;
using CloudyBank.PortableServices.Logs;


namespace CloudyBank.Web.Ria.ViewModels
{
    public class ReferentialDataViewModel : ViewModelBase
    {

        public ReferentialDataViewModel()
        {
            
        }

        #region Services

        private WCFCustomerProfileService _profileService;

        public WCFCustomerProfileService ProfileService
        {
            get {
                if (_profileService == null)
                {
                    _profileService = new WCFCustomerProfileServiceClient();
                }
                return _profileService; }
            set { _profileService = value; }
        }

        private WCFTagService _tagService;
        private WCFTagService TagService
        {
            get
            {
                if (_tagService == null)
                {
                    _tagService = new WCFTagServiceClient();
                }
                return _tagService;
            }
        }

        WCFAgencyService _agencyService;

        public WCFAgencyService AgencyService
        {
            get
            {
                if (_agencyService == null)
                {
                    _agencyService = new WCFAgencyServiceClient();
                }
                return _agencyService;
            }
            set { _agencyService = value; }
        }

        private WCFLogService _logService;

        public WCFLogService LogService
        {
            get {
                if (_logService == null)
                {
                    _logService = new WCFLogServiceClient();
                }
                return _logService; }
            set { _logService = value; }
        }

        
        #endregion

        #region CustomerProfiles

        public void LoadData()
        {
            ProfileService.BeginGetCustomerProfiles(EndGetCustomersProfile, null);
            TagService.BeginGetStandardTags(EndGetStandardTags, null);
            AgencyService.BeginGetAgencies(EndGetAgencies, null);
        }

        private ObservableCollection<CustomerProfileViewModel> _profiles;

        public ObservableCollection<CustomerProfileViewModel> Profiles
        {
            get { return _profiles; }
            set
            {
                _profiles = value;
                OnPropertyChanged(() => Profiles);
            }
        }

        private void EndGetCustomersProfile(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                try
                {
                    var profiles = ProfileService.EndGetCustomerProfiles(result);
                    if (profiles != null)
                    {
                        Profiles = new ObservableCollection<CustomerProfileViewModel>(profiles.Select(x => new CustomerProfileViewModel(x)));
                    }
                }
                catch (SecurityAccessDeniedException ex)
                {
                    LogService.BeginLogError(ex.Message, null, null);
                }
            }
        }
        #endregion

        #region Standard Tags
        private ObservableCollection<TagViewModel> _standardTags;

        public ObservableCollection<TagViewModel> StandardTags
        {
            get {
                if (_standardTags == null)
                {
                    _standardTags = new ObservableCollection<TagViewModel>();
                }
                return _standardTags; }
            set
            {
                _standardTags = value;
                OnPropertyChanged(() => StandardTags);
            }
        }

        public void EndGetStandardTags(IAsyncResult result)
        {
            try
            {
                StandardTags = new ObservableCollection<TagViewModel>(TagService.EndGetStandardTags(result).Select(x => new TagViewModel(x)));
            }
            catch (SecurityAccessDeniedException ex)
            {
                LogService.BeginLogError(ex.Message, null, null);
            }
            catch (CommunicationException ex)
            {
                LogService.BeginLogError(ex.Message, null, null);
            }
        }

        #endregion

        #region Agencies

        private void LoadAgencies()
        {
            AgencyService.BeginGetAgencies(EndGetAgencies, null);
        }

        private void EndGetAgencies(IAsyncResult result)
        {
            try
            {
                Agencies = new ObservableCollection<AgencyViewModel>(AgencyService.EndGetAgencies(result).Select(x => new AgencyViewModel(x)));
            }
            catch (SecurityAccessDeniedException ex)
            {
                LogService.BeginLogError(ex.Message, null, null);
            }
        }

        private ObservableCollection<AgencyViewModel> _agencies;

        public ObservableCollection<AgencyViewModel> Agencies
        {
            get { return _agencies; }
            set
            {
                _agencies = value;
                OnPropertyChanged(() => Agencies);
            }
        }
        #endregion
    }
}
