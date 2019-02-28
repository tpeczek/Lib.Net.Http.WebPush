using System;
using Microsoft.Extensions.Caching.Memory;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.AspNetCore.WebPush.Caching
{
    internal class MemoryVapidTokenCache : IVapidTokenCache
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryVapidTokenCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string Get(string audience)
        {
            if (!_memoryCache.TryGetValue(audience, out string token))
            {
                token = null;
            }

            return token;
        }

        public void Put(string audience, DateTimeOffset expiration, string token)
        {
            _memoryCache.Set(audience, token, expiration);
        }
    }
}
