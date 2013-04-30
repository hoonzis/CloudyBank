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

namespace CloudyBank.Web.Ria.Views.Windows
{
    public partial class ChooseReceiverWindow : ChildWindow
    {
        bool DoubleClick = false;

        private Object _selectedAccount = null;

        public Object SelectedAccount
        {
            get { return _selectedAccount; }
            set { _selectedAccount = value; }
        }
        private Object _selectedPartner = null;

        public Object SelectedPartner
        {
            get { return _selectedPartner; }
            set { _selectedPartner = value; }
        }


        public ChooseReceiverWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ChooseReceiverWindow_Loaded);
        }

        void ChooseReceiverWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            DoubleClick = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PartnerStack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedPartner = (sender as StackPanel).DataContext;
            if (DoubleClick)
                this.DialogResult = true;
            else
                DoubleClick = true;
        }

        private void AccountStack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedAccount = (sender as StackPanel).DataContext;
            if (DoubleClick)    
                this.DialogResult = true;
            else 
                DoubleClick = true;
        }
    }
}

