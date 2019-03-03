# PushServiceClient extensions for ASP.NET Core

The [`PushServiceClient`](../api/Lib.Net.Http.WebPush.PushServiceClient.html) extensions for ASP.NET Core provide an easy way to configure and create `PushServiceClient` instances in an app.

## Basic usage

The `PushServiceClient` factory (under the hood it uses `IHttpClientFactory`) can be registered by calling the [`AddPushServiceClient`](../api/Microsoft.Extensions.DependencyInjection.PushServiceClientServiceCollectionExtensions.html#Microsoft_Extensions_DependencyInjection_PushServiceClientServiceCollectionExtensions_AddPushServiceClient_Microsoft_Extensions_DependencyInjection_IServiceCollection_) extension method on the `IServiceCollection`, inside the `Startup.ConfigureServices` method. During registration there is an option to provide default authentication and configuration for a `PushServiceClient`.

```cs
services.AddPushServiceClient(options =>
{
    ...

    options.PublicKey = "<Application Server Public Key>";
    options.PrivateKey = "<Application Server Private Key>";
});
```

Once registered, code can accept a `PushServiceClient` anywhere services can be injected with dependency injection (DI).

```cs
internal class PushNotificationsDequeuer : IHostedService
{
    private readonly PushServiceClient _pushClient;

    public PushNotificationsDequeuer(PushServiceClient pushClient)
    {
        _pushClient = pushClient;
    }
}
```

## VAPID Tokens Caching

There is also an option to enable *VAPID* tokens caching by calling the [`AddMemoryVapidTokenCache`](../api/Microsoft.Extensions.DependencyInjection.PushServiceClientServiceCollectionExtensions.html#Microsoft_Extensions_DependencyInjection_PushServiceClientServiceCollectionExtensions_AddMemoryVapidTokenCache_Microsoft_Extensions_DependencyInjection_IServiceCollection_) or [`AddDistributedVapidTokenCache`](../api/Microsoft.Extensions.DependencyInjection.PushServiceClientServiceCollectionExtensions.html#Microsoft_Extensions_DependencyInjection_PushServiceClientServiceCollectionExtensions_AddDistributedVapidTokenCache_Microsoft_Extensions_DependencyInjection_IServiceCollection_) mehod.

```cs
services.AddMemoryCache();
services.AddMemoryVapidTokenCache();
services.AddPushServiceClient(options =>
{
    ...
});
```

Once a *VAPID* tokens cache is registered the factory will start using it automatically.