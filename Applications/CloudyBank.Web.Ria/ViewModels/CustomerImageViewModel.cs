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
using System.Windows.Media.Imaging;
using CloudyBank.Web.Ria.Technical.ImageTreatment;
using CloudyBank.PortableServices.Images;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class CustomerImageViewModel
    {
        public CustomerImageViewModel(CustomerImageDto dto)
        {
            CustomerImage = dto;
        }

        private CustomerImageDto _customerImage;

        public CustomerImageDto CustomerImage
        {
            get { return _customerImage; }
            set { _customerImage = value; }
        }

        private WriteableBitmap _image;

        public WriteableBitmap Image
        {
            get {
                if (_image == null)
                {
                    _image = ImageTreatment.ConvertToWB(CustomerImage.Data);
                }
                return _image; 
            }
            set { _image = value; }
        }

        public int Id
        {
            get { return CustomerImage.Id; }
        }
    }
}
