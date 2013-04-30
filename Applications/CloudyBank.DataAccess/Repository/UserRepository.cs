using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain;
using NHibernate;
using NHibernate.Linq;
using CloudyBank.CoreDomain.Security;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.CoreDomain.Advisors;

namespace CloudyBank.DataAccess.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public UserRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }
        
        public UserIdentity GetUserByIdentity(string identity)
        {
            ISession session = SessionFactory.GetCurrentSession();
            return session.Query<UserIdentity>().SingleOrDefault(x => x.Identification == identity);
        }


        public Role GetUserRole(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the use permission to the account specified by the ID. Method is called specially by the Security advice,
        /// when a method in bussiness layer is called and it is needed to check the role of the user which executes 
        /// the method to the account specified in the parameters of the method.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public Permission GetUserPermission(int accountId, string userIdentity)
        {
            ISession session = SessionFactory.GetCurrentSession();
            Account account = session.Load<Account>(accountId);

            UserIdentity identity = session.Query<UserIdentity>().SingleOrDefault(x => x.Identification == userIdentity);

            //if the user is advisor, than if it is a account of the client of the advisor, than the advisor can transfer, if not
            //he can only read.
            if (identity.UserType == UserType.Advisor)
            {
                Advisor advisor = session.Load<Advisor>(identity.Id);
                var allAccounts = advisor.Customers.SelectMany(x => x.RelatedAccounts);
                if (allAccounts.FirstOrDefault(x => x.Key.Id == accountId).Key != null)
                {
                    return Permission.Modify | Permission.Read | Permission.Write;
                }
                return Permission.Read;
            }
            else if (identity.UserType == UserType.CorporateCustomer || identity.UserType == UserType.IndividualCustomer)
            {
                //if it is a customer - than if is the account of the customer, than his permissions are set.
                //if not - no right
                Customer customer = session.Load<Customer>(identity.Id);
                if (customer.RelatedAccounts.ContainsKey(account))
                {
                    return customer.RelatedAccounts[account].Permission;
                }
                else
                {
                    return Permission.No;
                }
            }
            else
            {
                //some not expected user type
                return Permission.No;
            }
        }


        public UserType GetUserType(string login)
        {
            ISession session = SessionFactory.GetCurrentSession();
            var user = session.Query<UserIdentity>().SingleOrDefault(u => u.Identification == login);
            if (user != null)
            {
                return user.UserType;
            }
            return 0;
        }
    }
}
