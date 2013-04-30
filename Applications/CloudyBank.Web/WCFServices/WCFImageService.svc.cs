using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using CloudyBank.Core.Services;
using CloudyBank.Dto;
using CloudyBank.Services.ImageProc;
using CloudyBank.Services.Technical;


namespace CloudyBank.Web.WCFServices
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WCFImageService
    {

        private ICustomerServices _customerServices;

        public ICustomerServices CustomerServices
        {
            get {
                if (_customerServices == null)
                {
                    _customerServices = Global.GetObject<ICustomerServices>("CustomerServices");
                }
                return _customerServices; }
        }

        private IImageServices _imageServices;

        public IImageServices ImageServices
        {
            get {
                if (_imageServices == null)
                {
                    _imageServices = (IImageServices)Global.GetObject<IImageServices>("ImageServices");
                    ((ImageServices)ImageServices).HaarCascadeLocation = Utils.GetAppDataPath("haarcascade_frontalface_alt.xml");
                }
                return _imageServices; }
            
        }

        [OperationContract]
        public IList<CustomerImageDto> GetImages(int customerID)
        {
            return ImageServices.GetCustomerImages(customerID);//.Select(x => x.Data).ToList();
        }

        [OperationContract]
        public CustomerImageDto AddToSet(int userID,int[] pixels,int width, int height)
        {
            return ImageServices.AddImageToCustomer(pixels, userID, width,height);

        }

        [OperationContract]
        public String RecognizeClient(int[] pixels, int width, int height)
        {
            var customerID = ImageServices.RecognizeCustomer(pixels, width,height);
            return customerID;
            //return CustomerServices.GetCustomerByIdentity(customerID);
        }

        [OperationContract]
        public int RemoveClientImage(int id)
        {
            return ImageServices.RemoveCustomerImage(id);
        }

        //public int IsRecognized(String userLogin, int[] pixels)
        //{
        //    return 0;
        //}

        //public int Recognize(int[] pixels)
        //{
        //    return 0;
        //}

        

    }
}
