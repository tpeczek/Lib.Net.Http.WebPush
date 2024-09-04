using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Core;
using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.Azure.Functions.Worker.Extensions.WebPush
{
    [SupportsDeferredBinding]
    [SupportedTargetType(typeof(PushServiceClient))]
    internal class PushServiceConverter : IInputConverter
    {
        private static readonly Type TYPE_OF_PUSH_SERVICE_CLIENT = typeof(PushServiceClient);

        private readonly IHttpClientFactory _httpClientFactory;

        public PushServiceConverter(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
        {
            try
            {
                if (!CanConvert(context))
                {
                    return new(ConversionResult.Unhandled());
                }

                var modelBindingData = context?.Source as ModelBindingData;

                PushServiceClient pushServiceClient = CreatePushServiceClient(modelBindingData);

                return new(ConversionResult.Success(pushServiceClient));
            }
            catch (Exception ex)
            {
                return new(ConversionResult.Failed(ex));
            }
        }

        private static bool CanConvert(ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            if (context.TargetType != TYPE_OF_PUSH_SERVICE_CLIENT)
            {
                return false;
            }

            if (context.Source is not ModelBindingData bindingData)
            {
                return false;
            }

            if (bindingData.Source is not Constants.PUSH_SERVICE_EXTENSION_NAME)
            {
                throw new InvalidOperationException($"Unexpected binding source '{bindingData.Source}'. Only '{Constants.PUSH_SERVICE_EXTENSION_NAME}' is supported.");
            }

            if (bindingData.ContentType is not Constants.JSON_CONTENT_TYPE)
            {
                throw new InvalidOperationException($"Unexpected content-type '{bindingData.ContentType}'. Only '{Constants.JSON_CONTENT_TYPE}' is supported.");
            }

            return true;
        }

        private PushServiceClient CreatePushServiceClient(ModelBindingData bindingData)
        {
            var pushServiceModelBindingDataContent = bindingData.Content.ToObjectFromJson<PushServiceModelBindingDataContent>();

            PushServiceClient pushServiceClient = new PushServiceClient(_httpClientFactory.CreateClient())
            {
                DefaultAuthentication = new VapidAuthentication(pushServiceModelBindingDataContent.PublicKey, pushServiceModelBindingDataContent.PrivateKey)
                {
                    Subject = pushServiceModelBindingDataContent.Subject
                },
                AutoRetryAfter = pushServiceModelBindingDataContent.AutoRetryAfter,
                MaxRetriesAfter = pushServiceModelBindingDataContent.MaxRetriesAfter
            };

            if (pushServiceModelBindingDataContent.DefaultTimeToLive.HasValue)
            {
                pushServiceClient.DefaultTimeToLive = pushServiceModelBindingDataContent.DefaultTimeToLive.Value;
            }

            return pushServiceClient;
        }
    }
}
