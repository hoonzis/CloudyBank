using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;

namespace CloudyBank.Web.Security.OAuth
{
    public class CustomOAuthMessageFactory : OAuthServiceProviderMessageFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOAuthMessageFactory"/> class.
        /// </summary>
        /// <param name="tokenManager">The token manager instance to use.</param>
        public CustomOAuthMessageFactory(IServiceProviderTokenManager tokenManager)
            : base(tokenManager)
        {
        }

        public override IDirectedProtocolMessage GetNewRequestMessage(MessageReceivingEndpoint recipient, IDictionary<string, string> fields)
        {
            var message = base.GetNewRequestMessage(recipient, fields);

            // inject our own type here to replace the standard one
            if (message is UnauthorizedTokenRequest)
            {
                message = new RequestScopedTokenMessage(recipient, message.Version);
            }
            return message;
        }
    }
}
