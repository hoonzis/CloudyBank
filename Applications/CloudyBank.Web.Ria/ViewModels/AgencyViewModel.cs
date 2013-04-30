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
using Microsoft.Maps.MapControl;
using CloudyBank.PortableServices.Agencies;

//#if WINDOWS_PHONE
//#else
//using CloudyBank.Web.Ria.Agencies;
//#endif
namespace CloudyBank.Web.Ria.ViewModels
{
    public class AgencyViewModel : ViewModelBase
    {
        #region Agencies
        public AgencyViewModel(AgencyDto dto)
        {
            Location = new Location(dto.Lat, dto.Lng);
            Address = dto.Address;
            _closingHour = dto.ClosingHour;
            _openingHour = dto.OpeningHour;
            OnPropertyChanged(() => ClosingHour);
            OnPropertyChanged(() => OpeningHour);
        }


        private String _address;

        public String Address
        {
            get { return _address; }
            set { 
                _address = value;
                OnPropertyChanged(() => Address);
            }
        }

        private Location _location;

        public Location Location
        {
            get { return _location; }
            set { 
                _location = value;
                OnPropertyChanged(() => Location);
            }
        }

        private DateTime _openingHour;
        public String OpeningHour
        {
            get { return  String.Format("{0}h", _openingHour.Hour);; }
        }

        private DateTime _closingHour;
        public String ClosingHour { 
            get{
                return String.Format("{0}h", _closingHour.Hour);
            }
        }
        #endregion
    }
}
