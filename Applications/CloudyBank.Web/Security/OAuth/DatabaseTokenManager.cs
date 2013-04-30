using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth.ChannelElements;
using CloudyBank.Core.Services;
using CloudyBank.CoreDomain.Security;
using System.Security;
using System.Diagnostics.Contracts;
using CloudyBank.CoreDomain.Customers;
using CloudyBank.Web.OpenServices;
using CloudyBank.Services.Technical;


namespace CloudyBank.Web.Security.OAuth
{
    public class DatabaseTokenManager : IServiceProviderTokenManager
    {
        private IOAuthServices _oAuthServices;

        public IOAuthServices OAuthServices
        {
            get {
                if (_oAuthServices == null)
                {
                    _oAuthServices = Global.GetObject<IOAuthServices>("OAuthServices");
                }
                return _oAuthServices;
            }
        }

        public IServiceProviderAccessToken GetAccessToken(string token)
        {
            var authToken = OAuthServices.GetAccessToken(token);
            
            return new OAuthToken(authToken);
        }

        public IConsumerDescription GetConsumer(string consumerKey)
        {
            var consumer = OAuthServices.GetConsumer(consumerKey);
            if (consumer == null)
            {
                throw new SecurityException("No registered consumer with key: " + consumerKey);
            }
            return new OAuthConsumer(consumer);
        }

        public IServiceProviderRequestToken GetRequestToken(string token)
        {
            var authToken = OAuthServices.GetRequestToken(token);
            if (authToken == null)
            {
                throw new SecurityException("No token found: " + token);
            }
            return new OAuthToken(authToken);
        }

        public bool IsRequestTokenAuthorized(string requestToken)
        {
            var token = OAuthServices.GetRequestToken(requestToken);
            return token.State == CoreDomain.Security.AuthTokenState.AuthorizedRequestToken;
        }

        public void UpdateToken(IServiceProviderRequestToken token)
        {
            var authToken = OAuthServices.GetToken(token.Token);
            authToken.VerificationCode = token.VerificationCode;
            authToken.Callback = token.Callback.AbsoluteUri;
            authToken.Version = token.ConsumerVersion.ToString();
            authToken.Token = token.Token;
            authToken.VerificationCode = token.VerificationCode;
            _oAuthServices.UpdateToken(authToken);
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            var token = OAuthServices.GetRequestToken(requestToken);
            token.IssueDate = DateTime.UtcNow;
            token.State = CoreDomain.Security.AuthTokenState.AccessToken;
            token.Token = accessToken;
            token.TokenSecret = accessTokenSecret;

            OAuthServices.UpdateToken(token);
        }

        public string GetTokenSecret(string token)
        {
            var tokenInDb = OAuthServices.GetToken(token);
            return tokenInDb.TokenSecret;
        }

        public TokenType GetTokenType(string token)
        {
            var tokenInDb = OAuthServices.GetToken(token);
            if (tokenInDb == null)
            {
                return TokenType.InvalidToken;
            }
            else if (tokenInDb.State == CoreDomain.Security.AuthTokenState.AccessToken)
            {
                return TokenType.AccessToken;
            }
            else
            {
                return TokenType.RequestToken;
            }
        }

        public void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest request, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage response)
        {
            RequestScopedTokenMessage scopedRequest = (RequestScopedTokenMessage)request;
            var consumer = OAuthServices.GetConsumer(request.ConsumerKey);
            string scope = scopedRequest.Scope;

            var methodName = Utils.GetStringAfterLastSlash(scope);
            var openServiceType = typeof(DataService);
            var method = openServiceType.GetMethod(methodName);

            var scopeName = Global.ScopeNames[methodName];

            AuthToken newToken = new AuthToken
            {
                AuthConsumer = consumer,
                Token = response.Token,
                TokenSecret = response.TokenSecret,
                IssueDate = DateTime.UtcNow,
                Scope = scope,
                ScopeName = scopeName
            };

            OAuthServices.StoreNewRequestToken(newToken);
        }

        public void AuthorizeRequestToken(string requestToken, Customer customer)
        {
            var authToken = OAuthServices.GetRequestToken(requestToken);

            if (authToken == null)
            {
                throw new SecurityException("No unauthorized token found: " + requestToken);
            }
            if (authToken.State == AuthTokenState.UnauthorizedRequestToken)
            {
                authToken.State = AuthTokenState.AuthorizedRequestToken;
                authToken.Customer = customer;
                OAuthServices.UpdateToken(authToken);
            }
        }
    }
}
