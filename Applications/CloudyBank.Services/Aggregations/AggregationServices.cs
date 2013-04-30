using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Core.Aggregations;
using System.Transactions;

namespace CloudyBank.Services.Aggregations
{
    public class AggregationServices : IAggregationServices
    {
        private IRepository _repository;

        public AggregationServices(IRepository repository)
        {
            _repository = repository;
        }

        #region Account evolution
        public void UpdateBalancePoints(Account account)
        {
            if (account.BalancePoints.Count > 0)
            {
                var last = account.BalancePoints.OrderBy(x => x.Date).Last();

                //have to compare the dates! not datetimes
                var newOperations = account.Operations.Where(x => x.Date.Date.CompareTo(last.Date) > 0);
                var previousDate = last.Balance;
                CreateBalancePointsForOperations(newOperations, previousDate,account);
            }
            account.Balance = account.BalancePoints.Last().Balance;
        }

        public void UpdateAllBalancePoints()
        {
            var accounts = _repository.GetAll<Account>();
            foreach (var account in accounts)
            {
                UpdateBalancePoints(account);
            }
        }

        /// <summary>
        /// Helping method, iterates through operations and sums the values to compute the account evolution
        /// </summary>
        /// <param name="operations"></param>
        /// <param name="lastBalance"></param>
        /// <param name="account"></param>
        private void CreateBalancePointsForOperations(IEnumerable<Operation> operations, Decimal lastBalance,Account account)
        {
            
            var parDate = operations
                    .OrderBy(x => x.Date.Date)
                    .GroupBy(x => x.Date.Date)
                    .ToDictionary(x => x.Key, x => x.Sum(y => y.SignedAmount));

            using (TransactionScope scope = new TransactionScope())
            {

                foreach (var item in parDate)
                {
                    BalancePoint point = new BalancePoint { Date = item.Key, Balance = lastBalance + item.Value, Account = account }; // item.Value.Sum(x => x.SignedAmount) };
                    lastBalance = point.Balance;
                    _repository.Save(point);
                    account.BalancePoints.Add(point);
                }
                account.Balance = lastBalance;
                scope.Complete();
            }
        }

        /// <summary>
        /// Computes balance evolution of account - completely from beginning, if there was an evolution, erases the content
        /// </summary>
        /// <param name="account"></param>
        public void ComputeBalancePoints(Account account)
        {
            if (account.Operations.Count > 0)
            {
                account.BalancePoints = new List<BalancePoint>();
                CreateBalancePointsForOperations(account.Operations, 0, account);
            }
        }

        public void ComputeBalancesForAllAccounts()
        {
            var accounts = _repository.GetAll<Account>();
            foreach (var account in accounts)
            {
                ComputeBalancePoints(account);
            }
        }
        #endregion

        #region Tags aggregation

        public List<TagDepenses> AggregateProfileTagsFromDB(CustomerProfile profile)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();
            foreach (var customer in profile.Customers)
            {
                MapTagDepenses(depenses, customer.TagDepenses);
            }
            //profile done
            return depenses.Select(x => new TagDepenses() { Tag = x.Key, Depenses = x.Value.Depenses / profile.Customers.Count() }).ToList();

        }

        public List<TagDepenses> AggregateProfileTags(CustomerProfile profile)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();
            foreach (var customer in profile.Customers)
            {
                var tagDepenses = AggregateCustomer(customer);
                MapTagDepenses(depenses, tagDepenses);
                customer.TagDepenses = tagDepenses;
                _repository.Update(customer);
                _repository.Flush();
            }
            //profile done
            return depenses.Select(x => new TagDepenses() { Tag = x.Key, Depenses = x.Value.Depenses / profile.Customers.Count() }).ToList();

        }

        private void MapTagDepenses(Dictionary<Tag, TagDepenses> depenses, IList<TagDepenses> tagDepenses)
        {
            foreach (TagDepenses tagDepense in tagDepenses)
            {

                if (depenses.ContainsKey(tagDepense.Tag))
                {
                    depenses[tagDepense.Tag].Depenses += tagDepense.Depenses;
                }
                else
                {
                    depenses.Add(tagDepense.Tag, tagDepense);
                }
                _repository.Save(tagDepense);
            }
        }

        public List<TagDepenses> AggregateAccountTags(Account account)
        {
            //excluding null tags - this should never happen
            //while when new transactions is created, the tag is set to a new empty tag.

            var grouped = account.Operations.Where(x => x.SignedAmount < 0 && x.Tag!=null).GroupBy(x => x.Tag);
            var dict = grouped.ToDictionary(x => x.Key, y => y.Sum(x => x.SignedAmount));
            List<TagDepenses> depenses = dict.Select(x => new TagDepenses() { Tag = x.Key, Depenses = Math.Abs(x.Value) }).ToList();
            return depenses;
        }

        public List<TagDepenses> AggregateCustomer(Customer customer)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();

            foreach (Account account in customer.RelatedAccounts.Keys)
            {
                var tagDepenses = AggregateAccountTags(account);
                MapTagDepenses(depenses, tagDepenses);
                _repository.Update<Account>(account);
            }
            return depenses.Select(x => x.Value).ToList();
        }

        public List<TagDepenses> AggregateCustomerFromDB(Customer customer)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();

            foreach (Account account in customer.RelatedAccounts.Keys)
            {
                MapTagDepenses(depenses, account.TagDepenses);
            }
            return depenses.Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Assumes there is no data about repartition of tags in the database and recomputes all the data
        /// for all accounts, all customers and all customer profiles
        /// </summary>
        public void ReAggregateAllOperations()
        {


            var profiles = _repository.GetAll<CustomerProfile>().ToList();
            var customers = _repository.GetAll<Customer>().ToList();

            foreach (var customer in customers)
            {
                var profile = FindProfile(customer, profiles);
                if (profile != null)
                {
                    customer.CustomerProfile = profile;
                    profile.Customers.Add(customer);
                    _repository.Update(customer);
                    _repository.Update(profile);
                }
            }

            foreach (var profile in profiles)
            {
                var tagdepenses = AggregateProfileTags(profile);
                tagdepenses.ForEach(x => _repository.Save(x));

                profile.TagsRepartition = tagdepenses;
                _repository.Update(profile);
            }
            _repository.Flush();
        }

        #endregion

        /// <summary>
        /// Find a profile for a customer, based on the customers age and family situation
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public CustomerProfile FindProfile(Customer customer, IList<CustomerProfile> profiles)
        {
            var selectedProfile = (from profile in profiles
                                   where customer.GetAge() >= profile.LowAge && customer.GetAge() <= profile.HighAge && customer.Situation == profile.Situation 
                                   select profile).FirstOrDefault();
            return selectedProfile;
        }

    }
}
