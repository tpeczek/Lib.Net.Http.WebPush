using System;
using Microsoft.Extensions.Caching.Distributed;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.AspNetCore.WebPush.Caching
{
    internal class DistributedVapidTokenCache : IVapidTokenCache
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedVapidTokenCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public string Get(string audience)
        {
            return _distributedCache.GetString(audience);
        }

        public void Put(string audience, DateTimeOffset expiration, string token)
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(expiration);

            _distributedCache.SetString(audience, token, cacheEntryOptions);
        }
    }
}
