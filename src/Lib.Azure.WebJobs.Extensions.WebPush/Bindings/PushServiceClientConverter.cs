using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.Azure.WebJobs.Extensions.WebPush.Bindings
{
    internal class PushServiceClientConverter : IConverter<PushServiceAttribute, PushServiceClient>
    {
        private readonly PushServiceOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public PushServiceClientConverter(PushServiceOptions options, IHttpClientFactory httpClientFactory)
        {
            _options = options;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public PushServiceClient Convert(PushServiceAttribute attribute)
        {
            PushServiceClient pushServiceClient = new PushServiceClient(_httpClientFactory.CreateClient())
            {
                DefaultAuthentication = new VapidAuthentication(ResolveAuthenticationProperty(attribute.PublicKeySetting, _options?.PublicKey), ResolveAuthenticationProperty(attribute.PrivateKeySetting, _options?.PrivateKey))
                {
                    Subject = ResolveAuthenticationProperty(attribute.SubjectSetting, _options?.Subject)
                },
                AutoRetryAfter = attribute.AutoRetryAfter
            };

            if (attribute.DefaultTimeToLive.HasValue)
            {
                pushServiceClient.DefaultTimeToLive = attribute.DefaultTimeToLive.Value;
            }

            return pushServiceClient;
        }

        private static string ResolveAuthenticationProperty(string attributeValue, string optionsValue)
        {
            if (!String.IsNullOrEmpty(attributeValue))
            {
                return attributeValue;
            }

            return optionsValue;
        }
    }
}
