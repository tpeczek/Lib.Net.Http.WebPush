#if !NET451 && !NET461 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Text;
using System.Formats.Asn1;
using System.Security.Cryptography;

namespace Lib.Net.Http.WebPush.Internals
{
    internal class ES256Signer : IDisposable
    {
        private const string PRIVATE_DER_IDENTIFIER = "1.2.840.10045.3.1.7";
        private const string PRIVATE_PEM_KEY_PREFIX = "-----BEGIN EC PRIVATE KEY-----";
        private const string PRIVATE_PEM_KEY_SUFFIX = "-----END EC PRIVATE KEY-----";

        private readonly ECDsa _internalSigner;

        public ES256Signer(byte[] privateKey)
        {
            _internalSigner = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            _internalSigner.ImportFromPem(GetPrivateKeyPem(privateKey));
        }

        public byte[] GenerateSignature(string input)
        {
            return _internalSigner.SignData(Encoding.UTF8.GetBytes(input), HashAlgorithmName.SHA256);
        }

        public void Dispose()
        {
            _internalSigner?.Dispose();
        }

        private static ReadOnlySpan<char> GetPrivateKeyPem(byte[] privateKey)
        {
            AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
            asnWriter.PushSequence();
            asnWriter.WriteInteger(1);
            asnWriter.WriteOctetString(privateKey);
            asnWriter.PushSetOf(new Asn1Tag(TagClass.ContextSpecific, 0, true));
            asnWriter.WriteObjectIdentifier(PRIVATE_DER_IDENTIFIER);
            asnWriter.PopSetOf(new Asn1Tag(TagClass.ContextSpecific, 0, true));
            asnWriter.PopSequence();

            return PRIVATE_PEM_KEY_PREFIX + Environment.NewLine
                + Convert.ToBase64String(asnWriter.Encode()) + Environment.NewLine
                + PRIVATE_PEM_KEY_SUFFIX;
        }
    }
}
#endif