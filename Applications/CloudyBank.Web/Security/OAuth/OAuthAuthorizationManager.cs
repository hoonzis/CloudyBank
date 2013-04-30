using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DotNetOpenAuth.OAuth;
using CloudyBank.Core.Services;
using System.IdentityModel.Policy;
using System.ServiceModel.Security;
using System.Security.Principal;
using System.Diagnostics;
using Common.Logging;


namespace CloudyBank.Web.Security.OAuth
{
    /// <summary>
	/// A WCF extension to authenticate incoming messages using OAuth.
	/// </summary>
    public class OAuthAuthorizationManager : ServiceAuthorizationManager
    {
        private readonly ILog log = LogManager.GetCurrentClassLogger();

        public OAuthAuthorizationManager()
        {
        }


        private IOAuthServices _oAuthServices;

        public IOAuthServices OAuthServices
        {
            get
            {
                if (_oAuthServices == null)
                {
                    _oAuthServices = Global.GetObject<IOAuthServices>("OAuthServices");
                }
                return _oAuthServices;
            }
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (!base.CheckAccessCore(operationContext))
            {
                return false;
            }

            HttpRequestMessageProperty httpDetails = operationContext.RequestContext.RequestMessage.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            Uri requestUri = operationContext.RequestContext.RequestMessage.Properties.Via;
            Uri httpUriBeforeRewrite = HttpContext.Current.Request.Url;

            if (requestUri.AbsoluteUri!=httpUriBeforeRewrite.AbsoluteUri && httpDetails.Headers["Host"] != null)
            {
                string urlBeforeOverwrite = httpUriBeforeRewrite.AbsoluteUri.Replace(httpUriBeforeRewrite.Authority, httpDetails.Headers["Host"]);
                requestUri = new Uri(urlBeforeOverwrite);
            }

            ServiceProvider sp = Constants.CreateServiceProvider();
            try
            {
                var auth = sp.ReadProtectedResourceAuthorization(httpDetails, requestUri);
                if (auth != null)
                {
                    var accessToken = OAuthServices.GetAccessToken(auth.AccessToken);//  Global.AuthTokens.Single(token => token.Token == auth.AccessToken);

                    var principal = sp.CreatePrincipal(auth);
                    var policy = new OAuthPrincipalAuthorizationPolicy(principal);
                    var policies = new List<IAuthorizationPolicy> {
					    policy,
				    };

                    var securityContext = new ServiceSecurityContext(policies.AsReadOnly());
                    if (operationContext.IncomingMessageProperties.Security != null)
                    {
                        operationContext.IncomingMessageProperties.Security.ServiceSecurityContext = securityContext;
                    }
                    else
                    {
                        operationContext.IncomingMessageProperties.Security = new SecurityMessageProperty
                        {
                            ServiceSecurityContext = securityContext,
                        };
                    }

                    securityContext.AuthorizationContext.Properties["Identities"] = new List<IIdentity> {
					    principal.Identity,
				    };

                    // Only allow this method call if the access token scope permits it.
                    string[] scopes = accessToken.Scope.Split('|');

                    //originally this was ment to be used: operationContext.IncomingMessageHeaders.Action
                    //var action = operationContext.Host.BaseAddresses + operationContext.IncomingMessageProperties.Via.AbsolutePath;
                    //var action = "http://" + operationContext.IncomingMessageProperties.Via.Authority + operationContext.IncomingMessageProperties.Via.AbsolutePath;


                    //cut the url to the first "?", after the "?" there is bunch of OAuth stuff
                    //this way we can see whether the scope connected with this token is equal to the demanded service
                    var action = requestUri.AbsoluteUri.Substring(0, requestUri.AbsoluteUri.IndexOf("?"));

                    if (scopes.Contains(action))
                    {
                        return true;
                    }
                }
            }
            catch (ProtocolException ex)
            {
                log.Error(String.Format("Error in the communication protocol. {0}",ex.Message));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Unexpected error: {0}\n Accesing the url: {1}, which was before server rewrite: {2}",ex.Message,requestUri,httpUriBeforeRewrite));
            }

            return false;
        }
    }
}
