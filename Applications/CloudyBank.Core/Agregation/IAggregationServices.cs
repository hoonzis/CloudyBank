using System;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.CoreDomain.Customers;
using System.Collections.Generic;
namespace CloudyBank.Core.Aggregations
{
    public interface IAggregationServices
    {
        List<TagDepenses> AggregateAccountTags(Account account);
        List<TagDepenses> AggregateCustomer(Customer customer);
        List<TagDepenses> AggregateCustomerFromDB(Customer customer);
        List<TagDepenses> AggregateProfileTags(CustomerProfile profile);
        List<TagDepenses> AggregateProfileTagsFromDB(CustomerProfile profile);
        void ComputeBalancePoints(Account account);
        void ComputeBalancesForAllAccounts();
        CustomerProfile FindProfile(Customer customer, IList<CustomerProfile> profiles);
        void ReAggregateAllOperations();
        void UpdateBalancePoints(Account account);
        void UpdateAllBalancePoints();
    }
}
