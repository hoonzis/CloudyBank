using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CloudyBank.Web.Ria.ViewModels;
using System.Windows.Controls.DataVisualization;
using CloudyBank.PortableServices.CustomerProfiles;


namespace CloudyBank.Web.Ria.UserControls
{
    public partial class ProfileSelectorRdb : UserControl
    {
        private Range<int> _selectedAgeRange;
        private FamilySituation _selectedSituation;

        public List<Range<int>> AgeRanges
        {
            get
            {
                var data = CustomerProfiles.Select(x => new Range<int>(x.LowAge, x.HighAge)).Distinct();
                return data.ToList();
            }
        }

        public List<FamilySituation> Situations
        {
            get
            {
                var data = CustomerProfiles.Select(x => x.Situation).Distinct();
                return data.ToList();
            }
        }

        public CustomerProfileViewModel SelectedProfile
        {
            get { return (CustomerProfileViewModel)GetValue(SelectedProfileProperty); }
            set { SetValue(SelectedProfileProperty, value); }
        }

        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register("SelectedProfile", typeof(CustomerProfileViewModel), typeof(ProfileSelectorRdb), null);



        public IEnumerable<CustomerProfileViewModel> CustomerProfiles
        {
            get { return (IEnumerable<CustomerProfileViewModel>)GetValue(CustomerProfilesProperty); }
            set { SetValue(CustomerProfilesProperty, value); }
        }

        public static readonly DependencyProperty CustomerProfilesProperty =
            DependencyProperty.Register("CustomerProfiles", typeof(IEnumerable<CustomerProfileViewModel>), typeof(ProfileSelectorRdb), new PropertyMetadata(CustomerProfilesPropertyChanged));

        public static void CustomerProfilesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public ProfileSelectorRdb()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ProfileSelectorRdb_Loaded);
        }

        void ProfileSelectorRdb_Loaded(object sender, RoutedEventArgs e)
        {
            if (CustomerProfiles != null)
            {
                AgeRangeItemsControl.ItemsSource = AgeRanges;
                FamilySituationItemsControl.ItemsSource = Situations;
            }
        }

        //This method can be used if we are using Combobox instead of radiobutton
        //private void AgeRadioButton_Checked(object sender, RoutedEventArgs e)
        //{
        //    Range<int> range = (Range<int>)(sender as RadioButton).DataContext;
        //    _selectedAgeRange = range;
        //    UpdateSelectedProfile();
            
        //}

        private void UpdateSelectedProfile()
        {
            if (_selectedAgeRange != null)
            {
                if (_selectedAgeRange.HasData)
                {
                    SelectedProfile = CustomerProfiles.FirstOrDefault(x => (x.HighAge == _selectedAgeRange.Maximum && x.LowAge == _selectedAgeRange.Minimum && x.Situation == _selectedSituation));
                }
            }
        }

        //This can be used when Radtion buttons are used to visualize family situation
        //private void FamilySituationRDB_Checked(object sender, RoutedEventArgs e)
        //{
        //    var situation = (FamilySituation)(sender as RadioButton).DataContext;
        //    _selectedSituation = situation;
        //    UpdateSelectedProfile();
        //}

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Range<int> range = (Range<int>)(sender as ComboBox).SelectedItem;
            _selectedAgeRange = range;
            UpdateSelectedProfile();
        }

        private void FamilySituatioItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var situation = (FamilySituation)(sender as ComboBox).SelectedItem;
            _selectedSituation = situation;
            UpdateSelectedProfile();
        }
    }
}
