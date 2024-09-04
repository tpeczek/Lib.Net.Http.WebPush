# Lib.Net.Http.WebPush

[![NuGet Version](https://img.shields.io/nuget/v/Lib.Net.Http.WebPush?label=Lib.Net.Http.WebPush&logo=nuget)](https://www.nuget.org/packages/Lib.Net.Http.WebPush)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Lib.Net.Http.WebPush?label=⭳)](https://www.nuget.org/packages/Lib.Net.Http.WebPush)

[![NuGet Version](https://img.shields.io/nuget/v/Lib.AspNetCore.WebPush?label=Lib.AspNetCore.WebPush&logo=nuget)](https://www.nuget.org/packages/Lib.AspNetCore.WebPush)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Lib.AspNetCore.WebPush?label=⭳)](https://www.nuget.org/packages/Lib.AspNetCore.WebPush)

[![NuGet Version](https://img.shields.io/nuget/v/Lib.Azure.WebJobs.Extensions.WebPush?label=Lib.Azure.WebJobs.Extensions.WebPush&logo=nuget)](https://www.nuget.org/packages/Lib.Azure.WebJobs.Extensions.WebPush)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Lib.Azure.WebJobs.Extensions.WebPush?label=⭳)](https://www.nuget.org/packages/Lib.Azure.WebJobs.Extensions.WebPush)

[![NuGet Version](https://img.shields.io/nuget/v/Lib.Azure.Functions.Worker.Extensions.WebPush?label=Lib.Azure.Functions.Worker.Extensions.WebPush&logo=nuget)](https://www.nuget.org/packages/Lib.Azure.Functions.Worker.Extensions.WebPush)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Lib.Azure.Functions.Worker.Extensions.WebPush?label=⭳)](https://www.nuget.org/packages/Lib.Azure.Functions.Worker.Extensions.WebPush)

Lib.Net.Http.WebPush is a library which provides a [Web Push Protocol](https://tools.ietf.org/html/rfc8030) based client for Push Service. It provides support for [Voluntary Application Server Identification (VAPID) for Web Push](https://tools.ietf.org/html/rfc8292) and [Message Encryption for Web Push](https://tools.ietf.org/html/rfc8291).

Lib.AspNetCore.WebPush is a library which provides ASP.NET Core extensions for Web Push Protocol based client for Push Service.

Lib.Azure.WebJobs.Extensions.WebPush is a library which provides [Azure Functions](https://functions.azure.com/) in-process model and [Azure WebJobs](https://docs.microsoft.com/en-us/azure/app-service/web-sites-create-web-jobs) binding extensions for Web Push Protocol based client for Push Service.

Lib.Azure.Functions.Worker.Extensions.WebPush is a library which provides [Azure Functions](https://functions.azure.com/) isolated worker model extensions for Web Push Protocol based client for Push Service.

## Installation

You can install [Lib.Net.Http.WebPush](https://www.nuget.org/packages/Lib.Net.Http.WebPush), [Lib.AspNetCore.WebPush](https://www.nuget.org/packages/Lib.AspNetCore.WebPush), [Lib.Azure.WebJobs.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.WebJobs.Extensions.WebPush), and [Lib.Azure.Functions.Worker.Extensions.WebPush](https://www.nuget.org/packages/Lib.Azure.Functions.Worker.Extensions.WebPush) from NuGet.

```
PM>  Install-Package Lib.Net.Http.WebPush
```

```
PM>  Install-Package Lib.AspNetCore.WebPush
```

```
PM>  Install-Package Lib.Azure.WebJobs.Extensions.WebPush
```

```
PM>  Install-Package Lib.Azure.Functions.Worker.Extensions.WebPush
```

## Documentation

The documentation is available [here](https://tpeczek.github.io/Lib.Net.Http.WebPush/).

## Demos

There are several demo projects available:
- [Web Push Notifications in ASP.NET Core Web Application](https://github.com/tpeczek/Demo.AspNetCore.PushNotifications)
- [Web Push Notifications in ASP.NET Core-powered Angular Application](https://github.com/tpeczek/Demo.AspNetCore.Angular.PushNotifications)
- [Web Push Notifications in Azure Functions](https://github.com/tpeczek/Demo.Azure.Funtions.PushNotifications)

## Donating

My blog and open source projects are result of my passion for software development, but they require a fair amount of my personal time. If you got value from any of the content I create, then I would appreciate your support by [sponsoring me](https://github.com/sponsors/tpeczek) (either monthly or one-time).

## Copyright and License

Copyright © 2018 - 2024 Tomasz Pęczek

Licensed under the [MIT License](https://github.com/tpeczek/Lib.Net.Http.WebPush/blob/master/LICENSE.md)