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
using CloudyBank.Web.Ria.Technical.XamlSerialization;
using CloudyBank.PortableServices.CustomerProfiles;
using CloudyBank.PortableServices.TagDepenses;
using System.Linq;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class CustomerProfileViewModel : ViewModelBase
    {

        public CustomerProfileViewModel(CustomerProfileDto profile)
        {
            _customerProfile = profile;
            LoadTagDepenses();
        }

        #region Services

        private WCFTagDepensesService _tagDepensesService;

        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFTagDepensesService TagDepensesService
        {
            get
            {
                if (_tagDepensesService == null)
                {
                    _tagDepensesService = new WCFTagDepensesServiceClient();
                }
                return _tagDepensesService;
            }
            set { _tagDepensesService = value; }
        }
        #endregion

        private CustomerProfileDto _customerProfile;

        public int LowAge 
        {
            get {return _customerProfile.LowAge;}
            set
            {
                _customerProfile.HighAge = value;
                OnPropertyChanged(() => LowAge);
            }
        }

        public int HighAge
        {
            get { return _customerProfile.HighAge; }
            set
            {
                _customerProfile.HighAge = value;
                OnPropertyChanged(() => HighAge);
            }
        }

        public FamilySituation Situation
        {
            get { return _customerProfile.Situation; }
            set
            {
                _customerProfile.Situation = value;
                OnPropertyChanged(() => Situation);
            }
        }

        private void LoadTagDepenses()
        {
            TagDepensesService.BeginGetTagDepensesForProfile(_customerProfile.Id, EndGetTagDepensesForProfile, null);
        }

        private void EndGetTagDepensesForProfile(IAsyncResult result)
        {
            TagDepenses = new ObservableCollection<TagDepensesDto>(
                TagDepensesService.EndGetTagDepensesForProfile(result));
        }

        private ObservableCollection<TagDepensesDto> _tagDepenses;

        public ObservableCollection<TagDepensesDto> TagDepenses
        {
            get { return _tagDepenses; }
            set { 
                _tagDepenses = value;
                OnPropertyChanged(() => TagDepenses);
            }
        }
    }
}
