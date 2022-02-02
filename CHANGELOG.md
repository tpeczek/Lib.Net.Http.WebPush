## Lib.Net.Http.WebPush 3.1.0
### Additions and Changes
- Dropped support for .NET Core 2.1
- Added support for .NET 6.0

## Lib.Net.Http.WebPush 3.1.0
### Additions and Changes
- Dropped support for .NET Standard 1.6
- Removed dependency on Bouncy Castle for .NET 5
- `VapidAuthentication` now implements `IDisposable`

## Lib.Net.Http.WebPush 2.3.0
### Bug Fixes
- Fix for incorrect reuse of internally created `HttpContent`
- Fix for incorrect reuse of internally created `HttpRequestMessage`
### Additions and Changes
- Added context information to `PushServiceClientException` (thank you @techfg)
- Added setting for maximum limit of automatic retries in case of 429 Too Many Requests
- Exposed `AutoRetryAfter`, `DefaultTimeToLive`, and `MaxRetriesAfter` in Azure Functions binding

## Lib.Net.Http.WebPush 2.2.0
### Additions and Changes
- Added `HttpContent` based constructor for `PushMessage`

## Lib.Net.Http.WebPush 2.1.0
### Additions and Changes
- Automatic support for retries in case of 429 Too Many Requests

## Lib.Net.Http.WebPush 2.0.0
### Additions and Changes
- Strong named the assembly
- Changed default authentication scheme to VAPID

## Lib.Net.Http.WebPush 1.5.0
### Bug Fixes
- Fix for *Length Required* issue in case of push message delivery request to MS Edge

## Lib.Net.Http.WebPush 1.4.0
### Additions and Changes
- Added support for .NET Standard 2.0

## Lib.Net.Http.WebPush 1.3.0
### Bug Fixes
- Fix for push messages urgency
### Additions and Changes
- Added constructor which accepts instance of `HttpClient`
- Minor performance improvements

## Lib.Net.Http.WebPush 1.2.0
### Additions and Changes
- Added support for push messages topic
- Added support for push messages urgency

## Lib.Net.Http.WebPush 1.1.0
### Additions and Changes
- Added VAPID tokens caching capability
- Added support for both VAPID schemes