using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Dto;

namespace CloudyBank.Core.Services
{
    public interface IImageServices
    {
        List<CustomerImageDto> GetCustomerImages(int clientID);

        /// <summary>
        /// This method adds the image to the customer. Finds the face, resizes the faces and returns the DTO which 
        /// represents the image after treatment. If the face could not be found or if there was an error, there null is returned.
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="customerID"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        CustomerImageDto AddImageToCustomer(int[] pixels, int customerID, int width, int height);
        int RemoveCustomerImage(int id);
        String RecognizeCustomer(int[] pixels, int width, int height);
    }
}
