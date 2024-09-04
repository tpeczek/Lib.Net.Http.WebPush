using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace Lib.Azure.Functions.Worker.Extensions.WebPush
{
    /// <summary>
    /// Attribute used to configure a parameter as the input target for the PushService binding.
    /// </summary>
    /// <remarks>
    /// The method parameter type can be one of the following:
    /// <list type="bullet">
    /// <item><description><see cref="Net.Http.WebPush.PushServiceClient"/></description></item>
    /// </list>
    /// </remarks>
    [InputConverter(typeof(PushServiceConverter))]
    [ConverterFallbackBehavior(ConverterFallbackBehavior.Default)]
    public sealed class PushServiceInputAttribute : InputBindingAttribute
    {
        /// <summary>
        /// The application server public key.
        /// </summary>
        public string PublicKeySetting { get; set; }

        /// <summary>
        /// The application server private key.
        /// </summary>
        public string PrivateKeySetting { get; set; }

        /// <summary>
        /// The contact information for the application server.
        /// </summary>
        public string SubjectSetting { get; set; }

        /// <summary>
        /// The value indicating if client should automatically attempt to retry in case of 429 Too Many Requests.
        /// </summary>
        public bool AutoRetryAfter { get; set; } = true;

        /// <summary>
        /// The value indicating the maximum number of automatic attempts to retry in case of 429 Too Many Requests (&lt;= 0 means unlimited).
        /// </summary>
        public int MaxRetriesAfter { get; set; } = 0;

        /// <summary>
        /// The default time (in seconds) for which the message should be retained by push service. It will be used when <see cref="Net.Http.WebPush.PushMessage.TimeToLive"/> is not set.
        /// </summary>
        public int? DefaultTimeToLive { get; set; }
    }
}
