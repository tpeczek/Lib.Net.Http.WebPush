namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// The client keys shared as part of subscription.
    /// </summary>
    public enum PushEncryptionKeyName
    {
        /// <summary>
        /// The client P-256 public key for use in ECDH.
        /// </summary>
        P256DH,
        /// <summary>
        /// The client authentication secret.
        /// </summary>
        Auth
    }
}
