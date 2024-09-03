using System;

namespace Lib.Azure.WebJobs.Extensions.WebPush.Bindings
{
    internal class PushServiceParameterBindingDataContent
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public string Subject { get; set; }

        public bool AutoRetryAfter { get; set; }

        public int MaxRetriesAfter { get; set; }

        public int? DefaultTimeToLive { get; set; }

        public PushServiceParameterBindingDataContent(PushServiceAttribute attribute, PushServiceOptions options)
        {
            PublicKey = !String.IsNullOrEmpty(attribute.PublicKeySetting) ? attribute.PublicKeySetting : options?.PublicKey;
            PrivateKey = !String.IsNullOrEmpty(attribute.PrivateKeySetting) ? attribute.PrivateKeySetting : options?.PrivateKey;
            Subject = !String.IsNullOrEmpty(attribute.SubjectSetting) ? attribute.SubjectSetting : options?.Subject;
            AutoRetryAfter = attribute.AutoRetryAfter;
            MaxRetriesAfter = attribute.MaxRetriesAfter;
            DefaultTimeToLive = attribute.DefaultTimeToLive;
        }
    }
}
