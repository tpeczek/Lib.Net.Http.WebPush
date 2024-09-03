using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Lib.Azure.WebJobs.Extensions.WebPush.Bindings;
using Lib.Net.Http.WebPush;

namespace Lib.Azure.WebJobs.Extensions.WebPush.Config
{
    [Extension(Constants.PushServiceExtensionName)]
    internal class PushServiceExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly PushServiceOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public PushServiceExtensionConfigProvider(IOptions<PushServiceOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //PushServiceClient Bindings
            var bindingAttributeBindingRule = context.AddBindingRule<PushServiceAttribute>();
            bindingAttributeBindingRule.AddValidator(ValidateVapidAuthentication);

            bindingAttributeBindingRule.BindToInput<PushServiceClient>(typeof(PushServiceClientConverter), _options, _httpClientFactory);
            bindingAttributeBindingRule.BindToInput<ParameterBindingData>(CreateParameterBindingData);
        }

        private void ValidateVapidAuthentication(PushServiceAttribute attribute, Type paramType)
        {
            if (String.IsNullOrEmpty(_options.PublicKey) && String.IsNullOrEmpty(attribute.PublicKeySetting))
            {
                string attributeProperty = $"{nameof(PushServiceAttribute)}.{nameof(PushServiceAttribute.PublicKeySetting)}";
                string optionsProperty = $"{nameof(PushServiceOptions)}.{nameof(PushServiceOptions.PublicKey)}";
                throw new InvalidOperationException($"The application server public key must be set either via the {attributeProperty} property or via {optionsProperty}.");
            }

            if (String.IsNullOrEmpty(_options.PrivateKey) && String.IsNullOrEmpty(attribute.PrivateKeySetting))
            {
                string attributeProperty = $"{nameof(PushServiceAttribute)}.{nameof(PushServiceAttribute.PrivateKeySetting)}";
                string optionsProperty = $"{nameof(PushServiceOptions)}.{nameof(PushServiceOptions.PrivateKey)}";
                throw new InvalidOperationException($"The application server private key must be set either via the {attributeProperty} property or via {optionsProperty}.");
            }
        }

        internal ParameterBindingData CreateParameterBindingData(PushServiceAttribute attribute)
        {
            var pushServiceParameterBindingData = new PushServiceParameterBindingDataContent(attribute, _options);
            var pushServiceParameterBinaryData = new BinaryData(pushServiceParameterBindingData);
            var parameterBindingData = new ParameterBindingData("1.0", Constants.PushServiceExtensionName, pushServiceParameterBinaryData, "application/json");

            return parameterBindingData;
        }
    }
}
