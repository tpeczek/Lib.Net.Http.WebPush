namespace Lib.Azure.Functions.Worker.Extensions.WebPush
{
    internal class PushServiceModelBindingDataContent
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public string Subject { get; set; }

        public bool AutoRetryAfter { get; set; }

        public int MaxRetriesAfter { get; set; }

        public int? DefaultTimeToLive { get; set; }
    }
}
