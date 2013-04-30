using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Core.DataAccess;
using System.Transactions;
using CloudyBank.Dto;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;

namespace CloudyBank.Services.ImageProc
{
    public class ImageServices : IImageServices
    {

        private IRepository _repository;
        private IImageRepository _imageRepository;
        private String _haarCascadeLocation;

        public static int EIGEN_DISTANCE_THRESHOLD = 800;

        public String HaarCascadeLocation
        {
            get { return _haarCascadeLocation; }
            set { _haarCascadeLocation = value; }
        }

        

        public ImageServices(IRepository repository, IImageRepository imageRepository)
        {
            _repository = repository;
            _imageRepository = imageRepository;
        }

        public List<CustomerImageDto> GetCustomerImages(int customerID)
        {
            
            var images = _imageRepository.GetImagesForCustomer(customerID);
            return images.Select(x => new CustomerImageDto(){ Data = x.Data, Id = x.Id}).ToList();
            //var emguGrayScaled = images.Select(x => new Image<Gray, byte>(80,80) { Bytes = x.Data });

            //var bytes = emguGrayScaled.Select(x => ImageProcessingUtils.ConvertBitmapToByteArray(x.Bitmap,ImageFormat.Bmp));
            //return bytes.ToList();
        }

        

        public CustomerImageDto AddImageToCustomer(int[] pixels, int customerID, int width, int height)
        {
            Customer customer = _repository.Load<Customer>(customerID);
            CustomerImage image = new CustomerImage();

            var face = ImageProcessingUtils.DetectAndTrimFace(pixels, new Size(width,height), new Size(80,80), HaarCascadeLocation);
            if (face == null)
            {
                return null;
            }
            
            var equalized = ImageProcessingUtils.EqualizeHist(face);
            image.Customer = customer;

            image.Data = equalized.Bytes;
            image.Date = DateTime.Now;
            
            
            using (TransactionScope scope = new TransactionScope())
            {
                _repository.Save<CustomerImage>(image);
                scope.Complete();
            }

            return new CustomerImageDto() { Id = image.Id, Data = equalized.Bytes };
            
        }

        public int RemoveCustomerImage(int id)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var image = _repository.Load<CustomerImage>(id);
                    _repository.Delete<CustomerImage>(image);
                    scope.Complete();
                }
            }
            catch (DataAccessException)
            {
                return -1;
            }
            
            return id;
        }


        public String RecognizeCustomer(int[] pixels, int width, int height)
        {
            var face = ImageProcessingUtils.DetectAndTrimFace(pixels, new Size(width,height),new Size(80,80), HaarCascadeLocation);
            
            //if no or more than one face was presented in the picture
            if (face == null)
            {
                return null;
            }

            var equalized = ImageProcessingUtils.EqualizeHist(face);
            var recognizer = CreateRecognizer(0.001,EIGEN_DISTANCE_THRESHOLD );
            if (recognizer == null)
            {
                return null;
            }

            return recognizer.Recognize(equalized);
        }

        public EigenObjectRecognizer CreateRecognizer(double accuracy, int eigenDistanceThreshold)
        {
            var imagesTuple = _imageRepository.GetImagesLabelsTuples();

            List<Image<Gray, byte>> trainedImages = imagesTuple.Select(x => new Image<Gray, byte>(80, 80) { Bytes = x.Item1 }).ToList();
            List<String> labels = imagesTuple.Select(x => x.Item2).ToList();

            if (labels.Count == 0 || trainedImages.Count == 0)
            {
                return null;
            }

            //te accuracy is not important for eigenfaces algorithm
            //the number of iterations = the number of eigenfaces = count/3 - this is a parameter to play with
            MCvTermCriteria termCrit = new MCvTermCriteria(trainedImages.Count()/3, accuracy);


            EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                trainedImages.ToArray(),
                labels.ToArray(),
                eigenDistanceThreshold,
                ref termCrit);

            return recognizer;
        }
    }
}
