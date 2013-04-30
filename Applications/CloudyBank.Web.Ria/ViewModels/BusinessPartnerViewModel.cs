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
using CloudyBank.PortableServices.Partners;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class BusinessPartnerViewModel : ViewModelBase
    {
        public bool IsNew { get; set; }

        public BusinessPartnerViewModel()
        {
            _partner = new BusinessPartnerDto();
            IsNew = true;
        }

        public BusinessPartnerViewModel(BusinessPartnerDto partner)
        {
            _partner = partner;
            IsNew = false;
        }

        private BusinessPartnerDto _partner;

        public BusinessPartnerDto Partner
        {
            get { return _partner; }
            set { _partner = value;
            OnPropertyChanged(() => Partner);
            }
        }

        public int Id
        {
            get { return _partner.Id; }
            set
            {
                _partner.Id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public String Title
        {
            
            get { return _partner.Title; }
            set {
                _partner.Title = value;
                RemoveErrors("Title");

                if(String.IsNullOrWhiteSpace(value)){
                    AddError("Title", ViewModelsResources.TitleEmpty);
                }
                OnPropertyChanged(() => Title);
            }
        }

        public String Iban
        {
            get { return _partner.Iban; }
            set
            {
                _partner.Iban = value;
                OnPropertyChanged(() => Iban);
            }
        }

        public String Description
        {
            get { return _partner.Description; }
            set
            {
                _partner.Description = value;
                OnPropertyChanged(() => Description);
            }
        }
    }
}
