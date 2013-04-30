using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Security;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Dto;
using System.Transactions;

namespace CloudyBank.Services
{
    class OAuthServices : IOAuthServices
    {
        private IOAuthRepository _authRepository;
        private IRepository _repository;

        public OAuthServices(IRepository repository, IOAuthRepository authRepository)
        {
            _authRepository = authRepository;
            _repository = repository;
        }

        public AuthConsumer GetConsumer(string key)
        {            
            return _authRepository.GetConsumer(key);
        }

        public AuthToken GetRequestToken(string token)
        {
            
            var authToken = _authRepository.GetToken(token);
            if ((authToken.State == AuthTokenState.AuthorizedRequestToken) || (authToken.State == AuthTokenState.UnauthorizedRequestToken))
            {
                return authToken;
            }
            return null;
        }

        public AuthToken GetAccessToken(string token)
        {
            var tokenInDb = _authRepository.GetToken(token);
            if (tokenInDb.State == AuthTokenState.AccessToken)
            {
                return tokenInDb;
            }
            return null;
        }

        public void UpdateToken(AuthToken token)
        {
            _repository.Update(token);
            _repository.Flush();
        }

        public void StoreNewRequestToken(AuthToken token)
        {
            _repository.Save(token);
            _repository.Flush();
        }

        public AuthToken GetToken(string token)
        {
            return _authRepository.GetToken(token);
        }


        public bool InvalidateToken(int tokenID)
        {
            using (var scope = new TransactionScope())
            {
                var token = _repository.Load<AuthToken>(tokenID);
                if (token == null)
                {
                    throw new Exception("Error while obtaining token with ID: " + tokenID);
                }
                _repository.Delete<AuthToken>(token);
                scope.Complete();
                return true;
            }
        }

        public IList<TokenDto> GetCustomersTokens(int customerID)
        {
            var customer = _repository.Load<Customer>(customerID);
            if (customer == null)
            {
                throw new Exception("Error while obtaining customer with ID: " + customerID);
            }

            if (customer.Tokens != null)
            {

                var customers = customer.Tokens.Select(x => new TokenDto
                {
                    ApplicationName = x.AuthConsumer.Name,
                    ApplicationDescription = x.AuthConsumer.Description,
                    Id = x.Id,
                    Scope = x.ScopeName
                });

                return customers.ToList();
            }
            return new List<TokenDto>();
        }
    }
}
