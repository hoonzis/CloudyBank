using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.CoreDomain.Bank;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Services.PaymentClassification;

namespace CloudyBank.Services.Computations
{
    public class ComputationServices
    {
        private IRepository _repository;

        public ComputationServices(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Method which computes the Balance points of an account
        /// //TODO: Modify this to work only over certain time period
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private Dictionary<DateTime, BalancePoint> ComputeBalancePoints(Account account)
        {
            Dictionary<DateTime, BalancePoint> result = new Dictionary<DateTime, BalancePoint>();

            if (account.Operations.Count > 0)
            {

                var parDate = account.Operations
                    .OrderBy(x => x.Date.Date)
                    .GroupBy(x => x.Date.Date)
                    .ToDictionary(x => x.Key, x => x.ToList());



                decimal previousDate = 0;
                for (DateTime date = parDate.Keys.First(); date.CompareTo(DateTime.Now) <= 0; date = date.AddDays(1))
                {
                    BalancePoint point = null;
                    if (parDate.ContainsKey(date))
                    {
                        List<Operation> operationsToAdd = parDate[date];
                        point = new BalancePoint() { Date = date.Date, Balance = previousDate + operationsToAdd.Sum(x => x.SignedAmount) };
                    }
                    else
                    {
                        point = new BalancePoint() { Date = date, Balance = previousDate };
                    }
                    result.Add(date.Date, point);
                    previousDate = result[date].Balance;
                    _repository.SaveOrUpdate(point);
                }
            }
            return result;
        }

        private List<TagDepenses> AggregateProfileTags(CustomerProfile profile)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();
            foreach (var customer in profile.IndividualCustomers)
            {
                foreach (TagDepenses tagDepense in customer.TagDepenses)
                {

                    if (depenses.ContainsKey(tagDepense.Tag))
                    {
                        depenses[tagDepense.Tag].Depenses += tagDepense.Depenses;
                    }
                    else
                    {
                        depenses.Add(tagDepense.Tag, tagDepense);
                    }
                }
            }
            //profile done
            return depenses.Select(x => new TagDepenses() { Tag = x.Key, Depenses = x.Value.Depenses / profile.IndividualCustomers.Count() }).ToList();

        }

        private List<TagDepenses> AggregateAccountTags(Account account)
        {

            var grouped = account.Operations.Where(x => x.SignedAmount < 0).GroupBy(x => x.Tag);
            var dict = grouped.ToDictionary(x => x.Key, y => y.Sum(x => x.SignedAmount));
            List<TagDepenses> depenses = dict.Select(x => new TagDepenses() { Tag = x.Key, Depenses = x.Value }).ToList();
            return depenses;
        }

        private List<TagDepenses> AggregateCustomerTagDepenses(Customer customer)
        {
            Dictionary<Tag, TagDepenses> depenses = new Dictionary<Tag, TagDepenses>();

            foreach (Account account in customer.RelatedAccounts.Keys)
            {
                foreach (TagDepenses tagDepense in account.TagDepenses)
                {

                    if (depenses.ContainsKey(tagDepense.Tag))
                    {
                        depenses[tagDepense.Tag].Depenses += tagDepense.Depenses;
                    }
                    else
                    {
                        depenses.Add(tagDepense.Tag, tagDepense);
                    }
                }
            }
            return depenses.Select(x => x.Value).ToList();
        }

        private CustomerProfile FindProfile(IndividualCustomer customer, IList<CustomerProfile> profiles)
        {
            var selectedProfile = (from profile in profiles
                                   where customer.GetAge() >= profile.LowAge && customer.GetAge() <= profile.HighAge && profile.ChildernCount == customer.ChildernCount
                                   select profile).FirstOrDefault();
            return selectedProfile;
        }

        public void ComputeProfiles()
        {
            var accounts = _repository.GetAll<Account>();

            int constToFlush = 20;

            int i = 0;
            
            foreach (var account in accounts)
            {
                var tagdepenses = AggregateAccountTags(account);

                foreach (var tagdepense in tagdepenses)
                {
                    _repository.Save(tagdepense);
                }

                account.TagDepenses = tagdepenses;
                _repository.Update(account);
                if (i % constToFlush == 0)
                {
                    _repository.Flush();
                }
                i++;
            }
            _repository.Flush();

            var customers = _repository.GetAll<Customer>();

            foreach (var customer in customers)
            {
                var tagdepenses = AggregateCustomerTagDepenses(customer);

                foreach (var tagdepense in tagdepenses)
                {
                    _repository.Save(tagdepense);
                }

                customer.TagDepenses = tagdepenses;
                _repository.Update(customer);
                if (i % constToFlush == 0)
                {
                    _repository.Flush();
                }
            }
            _repository.Flush();

            var profiles = _repository.GetAll<CustomerProfile>();
            foreach (var profile in profiles)
            {
                var tagdepenses = AggregateProfileTags(profile);

                foreach (var tagdepense in tagdepenses)
                {
                    _repository.Save(tagdepense);
                }

                profile.TagsRepartition = tagdepenses;
                _repository.Update(profile);

            }
            _repository.Flush();

        }

        public void CategorizePayments(Account account)
        {
            //TODO:get the owner of the account,get his personal tags
            //get his personal rules
            //classify his spendings

            TextClassifier classifier = new TextClassifier();

            var operations = account.Operations;
            foreach (var operation in operations)
            {
                var tag = classifier.Categorize(operation.OppositeDescription);
                operation.Tag = tag;
                _repository.SaveOrUpdate(operation);
            }
        }
    }
}
