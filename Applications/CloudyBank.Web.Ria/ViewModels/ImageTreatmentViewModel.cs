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
using CloudyBank.Web.Ria.Technical.XamlSerialization;
using CloudyBank.MVVM;
using System.Windows.Media.Imaging;
using CloudyBank.Web.Ria.Components;
using System.Linq;
using CloudyBank.PortableServices.Images;
using System.Collections.ObjectModel;

namespace CloudyBank.Web.Ria.ViewModels
{
    public class ImageTreatmentViewModel : ViewModelBase
    {
        public int CustomerID { get; set; }
        #region Services
        private WCFImageService _imageService;
        [XamlSerializationVisibility(SerializationVisibility.Hidden)]
        public WCFImageService ImageService
        {
            get
            {
                if (_imageService == null)
                {
                    _imageService = ServicesFactory.GetObject<WCFImageService>();//, WCFImageServiceClient>();
                }
                return _imageService;
            }
            set { _imageService = value; }
        }

        #region Images
        ObservableCollection<CustomerImageViewModel> _images;

        public ObservableCollection<CustomerImageViewModel> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged(() => Images);
            }
        }
        #endregion

        public void GetCustomerImages()
        {
            InProgress = true;
            ImageService.BeginGetImages(CustomerID, EndGetCustomerImages, null);
        }

        public void EndGetCustomerImages(IAsyncResult result)
        {
            var images = ImageService.EndGetImages(result);

            //Instantiating WritableBitmap has to be performed from UI thread.
            //because te constructor of CustomerImageViewModel creates WritableBitmap - this action has to be delageted 
            //to the UI thread

            DelegateAction(() =>
            {
                Images = new ObservableCollection<CustomerImageViewModel>(images.Select(x => new CustomerImageViewModel(x)));// images.Select(x => ImageTreatment.ConvertToWB(x)));
            });
            InProgress = false;
        }

        public void AddImageToList(ImageCapturedArgs args)
        {
            InProgress = true;
            var bitmap = args.Bitmap.Resize(160, 120, WriteableBitmapExtensions.Interpolation.Bilinear);
            ImageService.BeginAddToSet(CustomerID, bitmap.Pixels.ToList(), bitmap.PixelWidth, bitmap.PixelHeight, EndAddImage, null);
        }

        public void EndAddImage(IAsyncResult result)
        {
            //Again - WritableBitmap has to be instantiated from UI thread
            DelegateAction(() =>
            {
                var pic = ImageService.EndAddToSet(result);
                if (pic == null)
                {
                    ErrorMessage = ViewModelsResources.NoFaceFound;
                    return;
                }

                Images.Add(new CustomerImageViewModel(pic));
            });
            InProgress = false;
        }

        private ICommand _addImage;

        public ICommand AddImage
        {
            get
            {
                if (_addImage == null)
                {
                    _addImage = new CommandBaseGeneric<ImageCapturedArgs>(AddImageToList, (x) => true);
                }
                return _addImage;
            }
        }

        private ICommand _removeImage;
        public ICommand RemoveImage
        {
            get
            {
                if (_removeImage == null)
                {
                    _removeImage = new CommandBaseGeneric<CustomerImageViewModel>(RemoveImageVM, (x) => true);
                }
                return _removeImage;
            }
        }

        public void RemoveImageVM(CustomerImageViewModel customerImage)
        {
            //if some image was selected
            if (customerImage != null)
            {
                ImageService.BeginRemoveClientImage(customerImage.Id, EndRemoveCustomerImage, null);
            }
        }

        public void EndRemoveCustomerImage(IAsyncResult result)
        {

            var id = ImageService.EndRemoveClientImage(result);
            if (id == -1)
            {
                ErrorMessage = ViewModelsResources.ImageDeleteIssue;
                return;
            }

            var image = Images.FirstOrDefault(x => x.Id == id);
            DelegateAction(() => Images.Remove(image));
        }

        #endregion

        private bool _inProgress;

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                OnPropertyChanged(() => InProgress);
            }
        }

        private String _errorMessage;
        public String ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }
    }
}
