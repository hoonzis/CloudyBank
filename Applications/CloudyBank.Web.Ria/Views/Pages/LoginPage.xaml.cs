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
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using CloudyBank.PortableServices.Images;
using CloudyBank.PortableServices.Users;


namespace CloudyBank.Web.Ria.Views
{
    public partial class LoginPage : Page
    {
        public event EventHandler LogedIn;

        private WCFImageService _imageService;

        public WCFImageService ImageService
        {
            get {
                if (_imageService == null)
                {
                    _imageService = new WCFImageServiceClient();
                }
                return _imageService; }
            set { _imageService = value; }
        }

        private WCFUserService _userService;

        public WCFUserService UserService
        {
            get {
                if (_userService == null)
                {
                    _userService = new WCFUserServiceClient();
                }
                return _userService; 
            }
            set { _userService = value; }
        }

        public LoginPage()
        {
            InitializeComponent();   
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            UserService.BeginLogin(LoginTextBox.Password, PasswordBox.Password, EndLogin, null);
        }

        public void EndLogin(IAsyncResult result){

            
            var isLoged = UserService.EndLogin(result);
            var handler = LogedIn;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        private void ImgTaker_ImageCaptured(object sender, Components.ImageCapturedArgs e)
        {

            var bitmap = e.Bitmap.Resize(160, 120, WriteableBitmapExtensions.Interpolation.Bilinear);
            ImageService.BeginRecognizeClient(bitmap.Pixels.ToList(),bitmap.PixelWidth, bitmap.PixelHeight,EndCustomerRecognized,null);
            InfoBoxTextBox.Text = "Trying to recognize you...";

        }

        private void EndCustomerRecognized(IAsyncResult result)
        {
            
            var customerID = ImageService.EndRecognizeClient(result);

            Dispatcher.BeginInvoke(() =>
            {
                if (customerID != null && customerID != String.Empty)
                {
                    LoginTextBox.Password = customerID;
                    InfoBoxTextBox.Text = "I have found you!";
                }
                else
                {
                    InfoBoxTextBox.Text = "Could not recognize you, sorry, you will have to enter your identification number";
                }
            });
            
        }

        private void RecognizeWithWebCamButton_Click(object sender, RoutedEventArgs e)
        {
            ImgTaker.Visibility = System.Windows.Visibility.Visible;
            ImgTaker.StartWebCam();
        }

    }
}
