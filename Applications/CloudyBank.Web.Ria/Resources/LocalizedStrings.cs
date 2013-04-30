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
using System.ComponentModel;
using CloudyBank.Web.Ria.Resources.Pages;

namespace CloudyBank.Web.Ria.Resources
{
    public class LocalizedStrings : INotifyPropertyChanged
    {
        
        private static readonly AgencyPageRes _agencyPageRes = new AgencyPageRes();
        private static readonly ProfilePageRes _profilePageRes = new ProfilePageRes();
        private static readonly CustomerPageRes _customerPageRes = new CustomerPageRes();
        private static readonly BudgetPageRes _budgetPageRes = new BudgetPageRes();
        private static readonly SettingsPageRes _settingsPageRes = new SettingsPageRes();
        private static readonly Common _common = new Common();
        

        public Common Common { get { return _common; } }
        public BudgetPageRes BudgetPageRes { get { return _budgetPageRes; } }
        public AgencyPageRes AgencyPageRes { get { return _agencyPageRes; } }
        public ProfilePageRes ProfilePageRes { get { return _profilePageRes; } }
        public CustomerPageRes CustomerPageRes { get { return _customerPageRes; } }
        public SettingsPageRes SettingsPageRes { get { return _settingsPageRes; } }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChange()
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
        }
    }
}
