# Performance Considerations

A typical application can be processing a very high number of *Push Messages*. Because of that it's important to consider performance best practices.

## Proper Instantiation

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) class is internally holding an instance of `HttpClient` class. As one can read in documentation:

> HttpClient is intended to be instantiated once and re-used throughout the life of an application. Instantiating an HttpClient class for every request will exhaust the number of sockets available under heavy loads.

Because of that (in order to avoid Improper Instantiation antipattern) a shared singleton instance of `PushServiceClient` should be created or a pool of reusable instances should be used.

## VAPID Tokens Caching

Generating *VAPID* tokens requires expensive cryptography. The structure of tokens allows for them to be cached per *Audience* (which means by *Push Service*) and *Application Server Keys* pair (for the token expiration period). This library provides such possibility through [`IVapidTokenCache`](../api/Lib.Net.Http.WebPush.Authentication.IVapidTokenCache.html). If an implementation of this interface will be provided to [`VapidAuthentication`](../api/Lib.Net.Http.WebPush.Authentication.VapidAuthentication.html) instance, it will result in tokens being cached.

Below is a sample implementation which uses [ASP.NET Core in-memory caching](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory).

```cs
public class MemoryVapidTokenCache : IVapidTokenCache
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
```

The usage of *Audience* as cache key means that intended context of this interface is a single *Application Server Keys* pair. If application is handling multiple *Application Server Keys* pairs it should provide a separate implementation for every pair and make sure those doesn't clash.