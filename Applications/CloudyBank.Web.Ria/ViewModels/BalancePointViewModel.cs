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
using CloudyBank.PortableServices.Accounts;

namespace CloudyBank.Web.Ria.ViewModels
{
    //This is just a simple wrapper over BalancePoint, mainly because AmCharts do not allow to
    //set StringFormat around category axis property. This get's anoying when DataTime is used
    public class BalancePointViewModel : ViewModelBase
    {
        public BalancePointDto BalancePoint { get; set; }

        public BalancePointViewModel()
        {
            BalancePoint = new BalancePointDto();
        }

        public BalancePointViewModel(BalancePointDto point)
        {
            BalancePoint = point;
        }

        public DateTime Date
        {
            get
            {
                return BalancePoint.Date;
            }
            set
            {
                BalancePoint.Date = value;
                OnPropertyChanged(() => BalancePoint);
            }
        }

        public String FormatedDate
        {
            get
            {
                return BalancePoint.Date.ToString("MM/dd");
            }
        }

        public Decimal Balance
        {
            get
            {
                return BalancePoint.Balance;
            }
            set
            {
                BalancePoint.Balance = value;
                OnPropertyChanged(() => BalancePoint);
            }
        }

        public int Id
        {
            get
            {
                return BalancePoint.Id;
            }
            set
            {
                BalancePoint.Id = value;
                OnPropertyChanged(() => Id);
            }
        }
    }
}
