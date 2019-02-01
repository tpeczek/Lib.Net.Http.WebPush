namespace Lib.Azure.WebJobs.Extensions.WebPush
{
    /// <summary>
    /// Options for Push Service binding extensions.
    /// </summary>
    public class PushServiceOptions
    {
        /// <summary>
        /// The application server public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// The application server private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// The contact information for the application server.
        /// </summary>
        public string Subject { get; set; }
    }
}
