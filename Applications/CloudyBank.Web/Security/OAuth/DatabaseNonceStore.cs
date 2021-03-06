using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging.Bindings;
using CloudyBank.Core.DataAccess;
using CloudyBank.CoreDomain.Security;

namespace CloudyBank.Web.Security.OAuth
{
    public class DatabaseNonceStore : INonceStore
    {
        private IRepository _repository;

        public IRepository Repository
        {
            get {
                if (_repository == null)
                {
                    _repository = Global.GetObject<IRepository>("GenericRepository");
                }
                return _repository;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseNonceStore"/> class.
        /// </summary>
        public DatabaseNonceStore()
        {
        }

        #region INonceStore Members

        /// <summary>
        /// Stores a given nonce and timestamp.
        /// </summary>
        /// <param name="context">The context, or namespace, within which the
        /// <paramref name="nonce"/> must be unique.
        /// The context SHOULD be treated as case-sensitive.
        /// The value will never be <c>null</c> but may be the empty string.</param>
        /// <param name="nonce">A series of random characters.</param>
        /// <param name="timestampUtc">The UTC timestamp that together with the nonce string make it unique
        /// within the given <paramref name="context"/>.
        /// The timestamp may also be used by the data store to clear out old nonces.</param>
        /// <returns>
        /// True if the context+nonce+timestamp (combination) was not previously in the database.
        /// False if the nonce was stored previously with the same timestamp and context.
        /// </returns>
        /// <remarks>
        /// The nonce must be stored for no less than the maximum time window a message may
        /// be processed within before being discarded as an expired message.
        /// This maximum message age can be looked up via the
        /// <see cref="DotNetOpenAuth.Configuration.MessagingElement.MaximumMessageLifetime"/>
        /// property, accessible via the <see cref="DotNetOpenAuth.Configuration.DotNetOpenAuthSection.Configuration"/>
        /// property.
        /// </remarks>
        public bool StoreNonce(string context, string nonce, DateTime timestampUtc)
        {
            AuthNonce authNonce = new AuthNonce { Context = context, Code = nonce, Timestamp = timestampUtc };
            Repository.Save(authNonce);
            Repository.Flush();
            return true;
        }

        #endregion
    }
}
