using System;
using System.Net;

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
        /// Creates new instance of <see cref="PushServiceClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception</param>
        /// <param name="statusCode">The status code of the push service response.</param>
        public PushServiceClientException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
