#if !NET451 && !NET461 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Formats.Asn1;
using System.Security.Cryptography;

namespace Lib.Net.Http.WebPush.Internals
{
    internal static class ECDHAgreementCalculator
    {
        private const string PRIVATE_DER_IDENTIFIER = "1.2.840.10045.3.1.7";
        private const string PUBLIC_DER_IDENTIFIER = "1.2.840.10045.2.1";

        private const string PUBLIC_PEM_KEY_PREFIX = "-----BEGIN PUBLIC KEY-----";
        private const string PUBLIC_PEM_KEY_SUFFIX = "-----END PUBLIC KEY-----";

        public static ECDHAgreement CalculateAgreement(byte[] otherPartyPublicKey, byte[] hmacKey)
        {
            using (ECDiffieHellman agreement = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
            {
                byte[] agreementPublicKey = GetAgreementPublicKey(agreement);

                byte[] sharedSecretHmac = agreement.DeriveKeyFromHmac(GetECDiffieHellmanPublicKey(otherPartyPublicKey), HashAlgorithmName.SHA256, hmacKey);

                return new ECDHAgreement(agreementPublicKey, sharedSecretHmac);
            }
        }

        private static byte[] GetAgreementPublicKey(ECDiffieHellman agreement)
        {
            ECParameters agreementParameters = agreement.ExportParameters(false);

            byte[] agreementPublicKey = new byte[agreementParameters.Q.X.Length + agreementParameters.Q.Y.Length + 1];

            agreementPublicKey[0] = 0x04;
            Array.Copy(agreementParameters.Q.X, 0, agreementPublicKey, 1, agreementParameters.Q.X.Length);
            Array.Copy(agreementParameters.Q.Y, 0, agreementPublicKey, agreementParameters.Q.X.Length + 1, agreementParameters.Q.Y.Length);

            return agreementPublicKey;
        }

        private static ECDiffieHellmanPublicKey GetECDiffieHellmanPublicKey(byte[] publicKey)
        {
            using (ECDiffieHellman ecdhAgreement = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256))
            {
                ecdhAgreement.ImportFromPem(GetPublicKeyPem(publicKey));

                return ecdhAgreement.PublicKey;
            }
        }

        private static ReadOnlySpan<char> GetPublicKeyPem(byte[] publicKey)
        {
            AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
            asnWriter.PushSequence();
            asnWriter.PushSequence();
            asnWriter.WriteObjectIdentifier(PUBLIC_DER_IDENTIFIER);
            asnWriter.WriteObjectIdentifier(PRIVATE_DER_IDENTIFIER);
            asnWriter.PopSequence();
            asnWriter.WriteBitString(publicKey);
            asnWriter.PopSequence();

            return PUBLIC_PEM_KEY_PREFIX + Environment.NewLine
                + Convert.ToBase64String(asnWriter.Encode()) + Environment.NewLine
                + PUBLIC_PEM_KEY_SUFFIX;
        }
    }
}
#endif