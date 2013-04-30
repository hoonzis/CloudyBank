using System;
using Microsoft.Pex.Framework;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Advisors;
using CloudyBank.CoreDomain.Bank;
using System.Collections.Generic;

namespace CloudyBank.CoreDomain.Customers
{
    /// <summary>A factory for CloudyBank.CoreDomain.Customers.Customer instances</summary>
    public static partial class CustomerFactory
    {
        /// <summary>A factory for CloudyBank.CoreDomain.Customers.Customer instances</summary>
        [PexFactoryMethod(typeof(Customer))]
        public static Customer Create(
            string value_s,
            string value_s1,
            Advisor value_advisor,
            CustomerProfile value_customerProfile,
            string value_s2,
            string value_s3,
            DateTime value_dt,
            FamilySituation value_i,
            string value_s4,
            string value_s5,
            IList<UserTag> value_iList,
            IList<PaymentEvent> value_iList1_,
            IList<BusinessPartner> value_iList2_,
            IList<TagDepenses> value_iList3_,
            IList<CustomerImage> value_iList4_,
            IList<AuthToken> value_iList5_,
            int value_i1_,
            string value_s6,
            string value_s7,
            DateTime value_dt1,
            DateTime value_dt2,
            string value_s8,
            UserType value_i2_
        )
        {
            Customer customer = new Customer();
            customer.Code = value_s;
            customer.PhoneNumber = value_s1;
            customer.Advisor = value_advisor;
            customer.CustomerProfile = value_customerProfile;
            customer.FirstName = value_s2;
            customer.LastName = value_s3;
            customer.BirthDate = value_dt;
            customer.Situation = value_i;
            customer.Password = value_s4;
            customer.PasswordSalt = value_s5;
            customer.Tags = value_iList;
            customer.PaymentEvents = value_iList1_;
            customer.Partners = value_iList2_;
            customer.TagDepenses = value_iList3_;
            customer.Images = value_iList4_;
            customer.Tokens = value_iList5_;
            ((UserIdentity)customer).Id = value_i1_;
            ((UserIdentity)customer).Identification = value_s6;
            ((UserIdentity)customer).Type = value_s7;
            ((UserIdentity)customer).ValidityEndDate = value_dt1;
            ((UserIdentity)customer).ValidityStartDate = value_dt2;
            ((UserIdentity)customer).Email = value_s8;
            ((UserIdentity)customer).UserType = value_i2_;
            return customer;
        }
    }
}
