using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Lib.Net.Http.WebPush.Internals
{
    internal static class ECKeyHelper
    {
        private const string PRIVATE_DER_IDENTIFIER = "1.2.840.10045.3.1.7";
        private const string PRIVATE_PEM_KEY_PREFIX = "-----BEGIN EC PRIVATE KEY-----\n";
        private const string PRIVATE_PEM_KEY_SUFFIX = "\n-----END EC PRIVATE KEY----";

        private const string PUBLIC_DER_IDENTIFIER = "1.2.840.10045.2.1";
        private const string PUBLIC_PEM_KEY_PREFIX = "-----BEGIN PUBLIC KEY-----\n";
        private const string PUBLIC_PEM_KEY_SUFFIX = "\n-----END PUBLIC KEY-----";

        private const string P256_CURVE_NAME = "P-256";
        private const string ECDH_ALGORITHM_NAME = "ECDH";

        internal static ECPrivateKeyParameters GetECPrivateKeyParameters(byte[] privateKey)
        {
            Asn1Object derSequence = new DerSequence(
                new DerInteger(1),
                new DerOctetString(privateKey),
                new DerTaggedObject(0, new DerObjectIdentifier(PRIVATE_DER_IDENTIFIER))
            );

            string pemKey = PRIVATE_PEM_KEY_PREFIX
                + Convert.ToBase64String(derSequence.GetDerEncoded())
                + PRIVATE_PEM_KEY_SUFFIX;

            PemReader pemKeyReader = new PemReader(new StringReader(pemKey));
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemKeyReader.ReadObject();

            return (ECPrivateKeyParameters)keyPair.Private;
        }

        internal static ECPublicKeyParameters GetECPublicKeyParameters(byte[] publicKey)
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

        internal static AsymmetricCipherKeyPair GenerateAsymmetricCipherKeyPair()
        {
            X9ECParameters ecParameters = NistNamedCurves.GetByName(P256_CURVE_NAME);
            ECDomainParameters ecDomainParameters = new ECDomainParameters(ecParameters.Curve, ecParameters.G, ecParameters.N, ecParameters.H, ecParameters.GetSeed());

            IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(ECDH_ALGORITHM_NAME);
            keyPairGenerator.Init(new ECKeyGenerationParameters(ecDomainParameters, new SecureRandom()));

            return keyPairGenerator.GenerateKeyPair();
        }
    }
}
