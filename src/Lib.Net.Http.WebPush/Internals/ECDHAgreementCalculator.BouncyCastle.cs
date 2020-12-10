#if NET451 || NET461 || NETSTANDARD2_0
using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto.Parameters;

namespace Lib.Net.Http.WebPush.Internals
{
    internal static class ECDHAgreementCalculator
    {
        private const string PRIVATE_DER_IDENTIFIER = "1.2.840.10045.3.1.7";
        private const string PUBLIC_DER_IDENTIFIER = "1.2.840.10045.2.1";

        private const string PUBLIC_PEM_KEY_PREFIX = "-----BEGIN PUBLIC KEY-----\n";
        private const string PUBLIC_PEM_KEY_SUFFIX = "\n-----END PUBLIC KEY-----";

        private const string P256_CURVE_NAME = "P-256";
        private const string ECDH_ALGORITHM_NAME = "ECDH";

        public static ECDHAgreement CalculateAgreement(byte[] otherPartyPublicKey, byte[] hmacKey)
        {
            AsymmetricCipherKeyPair agreementKeyPair = GenerateAsymmetricCipherKeyPair();
            byte[] agreementPublicKey = ((ECPublicKeyParameters)agreementKeyPair.Public).Q.GetEncoded(false);

            IBasicAgreement agreement = AgreementUtilities.GetBasicAgreement(ECDH_ALGORITHM_NAME);
            agreement.Init(agreementKeyPair.Private);

            byte[] sharedSecret = agreement.CalculateAgreement(GetECPublicKeyParameters(otherPartyPublicKey)).ToByteArrayUnsigned();

            return new ECDHAgreement(agreementPublicKey, HmacSha256(hmacKey, sharedSecret));
        }

        private static AsymmetricCipherKeyPair GenerateAsymmetricCipherKeyPair()
        {
            X9ECParameters ecParameters = NistNamedCurves.GetByName(P256_CURVE_NAME);
            ECDomainParameters ecDomainParameters = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H, ecParameters.GetSeed());

            IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(ECDH_ALGORITHM_NAME);
            keyPairGenerator.Init(new ECKeyGenerationParameters(ecDomainParameters, new SecureRandom()));

            return keyPairGenerator.GenerateKeyPair();
        }

        private static ECPublicKeyParameters GetECPublicKeyParameters(byte[] publicKey)
        {
            Asn1Object derSequence = new DerSequence(
                new DerSequence(new DerObjectIdentifier(PUBLIC_DER_IDENTIFIER), new DerObjectIdentifier(PRIVATE_DER_IDENTIFIER)),
                new DerBitString(publicKey)
            );

            string pemKey = PUBLIC_PEM_KEY_PREFIX
                + Convert.ToBase64String(derSequence.GetDerEncoded())
                + PUBLIC_PEM_KEY_SUFFIX;

            PemReader pemKeyReader = new PemReader(new StringReader(pemKey));
            return (ECPublicKeyParameters)pemKeyReader.ReadObject();
        }

        private static byte[] HmacSha256(byte[] key, byte[] value)
        {
            byte[] hash = null;

            using (HMACSHA256 hasher = new HMACSHA256(key))
            {
                hash = hasher.ComputeHash(value);
            }

            return hash;
        }
    }
}
#endif