namespace Lib.Net.Http.WebPush.Internals
{
    internal readonly struct ECDHAgreement
    {
        public byte[] PublicKey { get; }

        public byte[] SharedSecretHmac { get; }

        public ECDHAgreement(byte[] publicKey, byte[] sharedSecretHmac)
        {
            PublicKey = publicKey;
            SharedSecretHmac = sharedSecretHmac;
        }
    }
}
