#if NET451 || NET461 || NETSTANDARD2_0
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

namespace Lib.Net.Http.WebPush.Internals
{
    internal class ES256Signer : IDisposable
    {
        private const string PRIVATE_DER_IDENTIFIER = "1.2.840.10045.3.1.7";
        private const string PRIVATE_PEM_KEY_PREFIX = "-----BEGIN EC PRIVATE KEY-----\n";
        private const string PRIVATE_PEM_KEY_SUFFIX = "\n-----END EC PRIVATE KEY----";

        private readonly ECDsaSigner _internalSigner;

        public ES256Signer(byte[] privateKey)
        {
            _internalSigner = new ECDsaSigner();
            _internalSigner.Init(true, GetECPrivateKeyParameters(privateKey));
        }

        public byte[] GenerateSignature(string input)
        {
            byte[] hash = ComputeHash(input);

            BigInteger[] signature = _internalSigner.GenerateSignature(hash);

            byte[] jwtSignatureFirstSegment = signature[0].ToByteArrayUnsigned();
            byte[] jwtSignatureSecondSegment = signature[1].ToByteArrayUnsigned();

            int jwtSignatureSegmentLength = Math.Max(jwtSignatureFirstSegment.Length, jwtSignatureSecondSegment.Length);
            byte[] combinedJwtSignature = new byte[2 * jwtSignatureSegmentLength];
            ByteArrayCopyWithPadLeft(jwtSignatureFirstSegment, combinedJwtSignature, 0, jwtSignatureSegmentLength);
            ByteArrayCopyWithPadLeft(jwtSignatureSecondSegment, combinedJwtSignature, jwtSignatureSegmentLength, jwtSignatureSegmentLength);


            return combinedJwtSignature;
        }

        public void Dispose()
        { }

        private static byte[] ComputeHash(string input)
        {
            using (var sha256Hasher = SHA256.Create())
            {
                return sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        private static ECPrivateKeyParameters GetECPrivateKeyParameters(byte[] privateKey)
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

        private static void ByteArrayCopyWithPadLeft(byte[] sourceArray, byte[] destinationArray, int destinationIndex, int destinationLengthToUse)
        {
            if (sourceArray.Length != destinationLengthToUse)
            {
                destinationIndex += (destinationLengthToUse - sourceArray.Length);
            }

            Buffer.BlockCopy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
        }
    }
}
#endif