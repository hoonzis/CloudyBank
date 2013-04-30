using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth.ChannelElements;
using CloudyBank.CoreDomain.Security;
using System.Diagnostics.Contracts;

namespace CloudyBank.Web.Security.OAuth
{
    public class OAuthToken : IServiceProviderRequestToken, IServiceProviderAccessToken
    {
        public OAuthToken(AuthToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("Token passed to constructor of OAuthToken cannot be null");
            }
            Token = token;
        }

        public OAuthToken()
        {
            Token = new AuthToken();
        }

        public AuthToken Token {get;set;}

        #region IServiceProviderRequestToken

        Uri IServiceProviderRequestToken.Callback
        {
            get
            {
                return new Uri(Token.Callback);
            }
            set
            {
                if (value != null)
                {
                    Token.Callback = value.AbsoluteUri;
                }
            }
        }

        string IServiceProviderRequestToken.ConsumerKey
        {
            get { return Token.AuthConsumer.ConsumerKey; }
        }

        Version IServiceProviderRequestToken.ConsumerVersion
        {
            get
            {
                if (Token == null || Token.Version == null)
                {
                    throw new ArgumentNullException("The Token or the Version are null");
                }
                return new Version(Token.Version);
            }
            set
            {
                Token.Version = value.ToString();
            }
        }

        DateTime IServiceProviderRequestToken.CreatedOn
        {
            
            get {
                return Token.IssueDate.ToLocalTime(); }
        }

        string IServiceProviderRequestToken.Token
        {
            get { return Token.Token; }
        }

        string IServiceProviderRequestToken.VerificationCode
        {
            get
            {
                return Token.VerificationCode;
            }
            set
            {
                Token.VerificationCode = value;
            }
        }

        #endregion

        #region IServiceProviderAccessToken

        DateTime? IServiceProviderAccessToken.ExpirationDate
        {
            get { return Token.ExpirationDate; }
        }

        string[] IServiceProviderAccessToken.Roles
        {
            get { return Token.Roles; }
        }

        string IServiceProviderAccessToken.Token
        {
            get { return Token.Token; }
        }

        string IServiceProviderAccessToken.Username
        {
            get {
                if (Token.Customer == null)
                {
                    throw new ArgumentNullException("Token does not have assigned user");
                }
                return Token.Customer.Identification; 
            }
        }
        #endregion
    }
}
