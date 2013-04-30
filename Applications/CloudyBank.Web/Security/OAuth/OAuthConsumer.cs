using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth.ChannelElements;
using CloudyBank.CoreDomain.Security;
using System.Security.Cryptography.X509Certificates;

namespace CloudyBank.Web.Security.OAuth
{
    public class OAuthConsumer : IConsumerDescription
    {
        public OAuthConsumer()
        {
            Consumer = new AuthConsumer();
        }

        public OAuthConsumer(AuthConsumer consumer)
        {
            Consumer = consumer;
        }

        //db class which backsup this consumerdescription

        private AuthConsumer _consumer;
        public AuthConsumer Consumer
        {
            get
            {
                if (_consumer == null)
                {
                    _consumer = new AuthConsumer();
                }
                return _consumer;
            }
            set
            {
                _consumer = value;
            }
        }


        Uri IConsumerDescription.Callback
        {
            get { return new Uri(Consumer.Callback); }
        }

        X509Certificate2 IConsumerDescription.Certificate
        {
            //If this would be used, how to store it in the DB?
            get { return null; }
        }

        string IConsumerDescription.Key
        {
            get { return Consumer.ConsumerKey; }
        }

        string IConsumerDescription.Secret
        {
            get { return Consumer.Secret; }
        }

        DotNetOpenAuth.OAuth.VerificationCodeFormat IConsumerDescription.VerificationCodeFormat
        {
            get { return (DotNetOpenAuth.OAuth.VerificationCodeFormat)Consumer.VerificationCodeFormat; }
        }

        int IConsumerDescription.VerificationCodeLength
        {
            get { return Consumer.VerificationCodeLength; }
        }
    }
}
