using System;
using System.Net;
using System.Net.Http.Headers;

namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// An exception representing requesting <see cref="PushMessage"/> delivery failure based on push service response.
    /// </summary>
    public class PushServiceClientException : Exception
    {
        /// <summary>
        /// Gets or sets the status code of the push service response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets or sets the HTTP Response Headers of the push service response.
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        /// Gets or sets the PushSubscription that initiated the push service response.
        /// </summary>
        public PushSubscription PushSubscription { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="PushServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        /// <param name="statusCode">The status code of the push service response.</param>
        /// <param name="headers">The headers of the push service response.</param>
        /// <param name="pushSubscription">The push subscription that initiated the push service response.</param>
        public PushServiceClientException(string message, HttpStatusCode statusCode, HttpResponseHeaders headers, PushSubscription pushSubscription)
            : base(message)
        {
            StatusCode = statusCode;
            Headers = headers;
            PushSubscription = pushSubscription;
        }
    }
}
