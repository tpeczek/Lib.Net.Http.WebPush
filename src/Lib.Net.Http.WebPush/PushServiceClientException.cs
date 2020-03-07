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
        #region Properties
        /// <summary>
        /// Gets the status code of the push service response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the headers of the push service response.
        /// </summary>
        public HttpResponseHeaders Headers { get; }

        /// <summary>
        /// Gets the body of the push service response.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets the <see cref="WebPush.PushSubscription"/> that initiated the push service request.
        /// </summary>
        public PushSubscription PushSubscription { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new instance of <see cref="PushServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        /// <param name="statusCode">The status code of the push service response.</param>
        public PushServiceClientException(string message, HttpStatusCode statusCode)
            : this(message, statusCode, null, null, null)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="PushServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        /// <param name="statusCode">The status code of the push service response.</param>
        /// <param name="headers">The headers of the push service response.</param>
        /// <param name="body">The body of the push service response.</param>
        /// <param name="pushSubscription">The <see cref="WebPush.PushSubscription"/> that initiated the push service request.</param>
        public PushServiceClientException(string message, HttpStatusCode statusCode, HttpResponseHeaders headers, string body, PushSubscription pushSubscription)
            : base(message)
        {
            StatusCode = statusCode;
            Headers = headers;
            Body = body;
            PushSubscription = pushSubscription;
        }
        #endregion
    }
}
