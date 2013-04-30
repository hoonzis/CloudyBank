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
using System.Windows.Navigation;
using CloudyBank.Web.Ria.ViewModels;
using SLaB.Utilities.Xaml.Serializer;
using CloudyBank.Web.Ria.Technical;
using System.IO;
using System.Reflection;
using System.Collections;

namespace CloudyBank.Web.Ria.Views.Pages
{
    public partial class SettingsPage : Page
    {
        private IOService _ioService;

        public IOService IoService
        {
            get
            {
                if (_ioService == null)
                {
                    _ioService = new FDService();
                }
                return _ioService;
            }
            set { _ioService = value; }
        }

        public SettingsPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ImageTreatmentViewModel vm = new ImageTreatmentViewModel();
            var customer = this.DataContext as CustomerViewModel;

            if (customer.Customer == null)
            {
                return;
            }

            vm.CustomerID = customer.Customer.Id;
            vm.GetCustomerImages();
            ImageTreatmentBorder.DataContext = vm;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ImgTaker.ReleaseCaptureSource();
            base.OnNavigatedFrom(e);
        }

        //private void GenerateButton_Click(object sender, RoutedEventArgs e)
        //{

        //    List<Type> types = new List<Type>();
        //    types.Add(typeof(AccountViewModel));
        //    types.Add(typeof(OperationViewModel));

        //    SaveFileDialog sFile = new SaveFileDialog();
        //    sFile.ShowDialog();
        //    sFile.DefaultExt = "xaml";
        //    try
        //    {
        //        using (Stream stream = sFile.OpenFile())
        //        {
        //            CustomerViewModel customer = this.DataContext as CustomerViewModel;
        //            XamlSerializer ser = new XamlSerializer();
        //            String data = String.Empty;
        //            if ((bool)AccountVMRB.IsChecked)
        //            {
        //                data = ser.Serialize(customer.Accounts[0]);
        //            }
        //            else
        //            {
        //                data = ser.Serialize(customer);
        //            }

        //            StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8);
        //            sw.Write(data);
        //            sw.Close();

        //            stream.Close();
        //        }
        //    }

        //    catch (InvalidOperationException)
        //    {
        //        //no file selected
        //    }
        //}

        private void StartWebCamButton_Click(object sender, RoutedEventArgs e)
        {
            ImgTaker.Visibility = System.Windows.Visibility.Visible;
            ImgTaker.StartWebCam();
            StartWebCamButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        //private void Image_KeyDown(object sender, KeyEventArgs e)
        //{
        //    var vm = (CustomerViewModel)this.DataContext;
        //    MessageBox.Show(e.Key.ToString());
        //}
    }
}
