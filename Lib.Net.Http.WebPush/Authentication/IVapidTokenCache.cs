using System;

namespace Lib.Net.Http.WebPush.Authentication
{
    /// <summary>
    /// Represents cache for <see cref="VapidAuthentication"/> tokens.
    /// </summary>
    public interface IVapidTokenCache
    {
        /// <summary>
        /// Puts token into cache.
        /// </summary>
        /// <param name="audience">The origin of the push resource (cache key).</param>
        /// <param name="expiration">The token expiration.</param>
        /// <param name="token">The token.</param>
        void Put(string audience, DateTimeOffset expiration, string token);

        /// <summary>
        /// Gets token from cache.
        /// </summary>
        /// <param name="audience">The origin of the push resource (cache key).</param>
        /// <returns>The cached token or null if token was not present in cache.</returns>
        string Get(string audience);
    }
}
