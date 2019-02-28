using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.AspNetCore.WebPush
{
    /// <summary>
    /// The options for <see cref="Lib.Net.Http.WebPush.PushServiceClient"/>
    /// </summary>
    public class PushServiceClientOptions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the contact information for the application server.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the Application Server Public Key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the Application Server Private Key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the time after which the authentication token expires (in seconds).
        /// </summary>
        public int? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="VapidAuthenticationScheme"/> to be used.
        /// </summary>
        public VapidAuthenticationScheme DefaultAuthenticationScheme { get; set; } = VapidAuthenticationScheme.Vapid;

        /// <summary>
        /// Gets or sets the value indicating if client should automatically attempt to retry in case of 429 Too Many Requests.
        /// </summary>
        public bool AutoRetryAfter { get; set; } = true;

        /// <summary>
        /// Gets or sets the default time (in seconds) for which the message should be retained by push service. It will be used when <see cref="PushMessage.TimeToLive"/> is not set.
        /// </summary>
        public int? DefaultTimeToLive { get; set; }
        #endregion
    }
}
