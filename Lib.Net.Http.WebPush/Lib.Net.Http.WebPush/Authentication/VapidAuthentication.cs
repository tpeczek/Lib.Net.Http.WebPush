using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Lib.Net.Http.WebPush.Internals;

namespace Lib.Net.Http.WebPush.Authentication
{
    /// <summary>
    /// Class which provides Voluntary Application Server Identification (VAPID) headers values.
    /// </summary>
    public class VapidAuthentication
    {
        #region Structures
        /// <summary>
        /// Structure providing values for headers used in case of <see cref="VapidAuthenticationScheme.WebPush"/>.
        /// </summary>
        public readonly struct WebPushSchemeHeadersValues
        {
            /// <summary>
            /// Gets the <see cref="System.Net.Http.Headers.AuthenticationHeaderValue"/> parameter.
            /// </summary>
            public string AuthenticationHeaderValueParameter { get; }

            /// <summary>
            /// Gets the Crypto-Key header value.
            /// </summary>
            public string CryptoKeyHeaderValue { get; }

            internal WebPushSchemeHeadersValues(string authenticationHeaderValueParameter, string cryptoKeyHeaderValue)
                : this()
            {
                AuthenticationHeaderValueParameter = authenticationHeaderValueParameter;
                CryptoKeyHeaderValue = cryptoKeyHeaderValue;
            }
        }
        #endregion

        #region Fields
        private const string URI_SCHEME_HTTPS = "https";
        private const string AUDIENCE_CLAIM = "aud";
        private const string EXPIRATION_CLAIM = "exp";
        private const string SUBJECT_CLAIM = "sub";
        private const char JWT_SEPARATOR = '.';

        private const string P256ECDSA_PREFIX = "p256ecdsa=";
        private const string VAPID_AUTHENTICATION_HEADER_VALUE_PARAMETER_FORMAT = "t={0}, k={1}";

        private const int DEFAULT_EXPIRATION = 43200;
        private const int MAXIMUM_EXPIRATION = 86400;

        private string _subject;
        private string _publicKey;
        private string _privateKey;
        private ECPrivateKeyParameters _privateSigningKey;
        private int _expiration;

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private static readonly Dictionary<string, string> _jwtHeader = new Dictionary<string, string>
        {
            { "typ", "JWT" },
            { "alg", "ES256" }
        };
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the contact information for the application server.
        /// </summary>
        public string Subject
        {
            get { return _subject; }

            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (!value.StartsWith("mailto:"))
                    {
                        if (!Uri.IsWellFormedUriString(value, UriKind.Absolute) || ((new Uri(value)).Scheme != URI_SCHEME_HTTPS))
                        {
                            throw new ArgumentException(nameof(Subject), "Subject should include a contact URI for the application server as either a 'mailto: ' (email) or an 'https:' URI");
                        }
                    }

                    _subject = value;
                }
                else
                {
                    _subject = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Application Server Public Key.
        /// </summary>
        public string PublicKey
        {
            get { return _publicKey; }

            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(PublicKey));
                }

                byte[] decodedPublicKey = UrlBase64Converter.FromUrlBase64String(value);
                if (decodedPublicKey.Length != 65)
                {
                    throw new ArgumentException(nameof(PublicKey), "VAPID public key must be 65 bytes long");
                }

                _publicKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the Application Server Private Key.
        /// </summary>
        public string PrivateKey
        {
            get { return _privateKey; }

            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(PrivateKey));
                }

                byte[] decodedPrivateKey = UrlBase64Converter.FromUrlBase64String(value);
                if (decodedPrivateKey.Length != 32)
                {
                    throw new ArgumentException(nameof(PrivateKey), "VAPID private key should be 32 bytes long");
                }

                _privateKey = value;
                _privateSigningKey = ECKeyHelper.GetECPrivateKeyParameters(decodedPrivateKey);
            }
        }

        /// <summary>
        /// Gets or sets the time after which the authentication token expires (in seconds).
        /// </summary>
        public int Expiration
        {
            get { return _expiration; }

            set
            {
                if ((value <= 0) || (value > MAXIMUM_EXPIRATION))
                {
                    throw new ArgumentOutOfRangeException(nameof(Expiration), "Expiration must be a number of seconds not longer than 24 hours");
                }

                _expiration = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new instance of <see cref="VapidAuthentication"/> class.
        /// </summary>
        /// <param name="publicKey">The Application Server Public Key.</param>
        /// <param name="privateKey">The Application Server Private Key.</param>
        public VapidAuthentication(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;

            _expiration = DEFAULT_EXPIRATION;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets <see cref="System.Net.Http.Headers.AuthenticationHeaderValue"/> parameter for <see cref="VapidAuthenticationScheme.Vapid"/>.
        /// </summary>
        /// <param name="audience">The origin of the push resource.</param>
        /// <returns>The <see cref="System.Net.Http.Headers.AuthenticationHeaderValue"/> parameter for <see cref="VapidAuthenticationScheme.Vapid"/>.</returns>
        public string GetVapidSchemeAuthenticationHeaderValueParameter(string audience)
        {
            return String.Format(VAPID_AUTHENTICATION_HEADER_VALUE_PARAMETER_FORMAT, GetToken(audience), _publicKey);
        }

        /// <summary>
        /// Gets values for headers used in case of <see cref="VapidAuthenticationScheme.WebPush"/>.
        /// </summary>
        /// <param name="audience">The origin of the push resource.</param>
        /// <returns>The values for headers used in case of <see cref="VapidAuthenticationScheme.WebPush"/>.</returns>
        public WebPushSchemeHeadersValues GetWebPushSchemeHeadersValues(string audience)
        {
            return new WebPushSchemeHeadersValues(GetToken(audience), P256ECDSA_PREFIX + _publicKey);
        }

        private string GetToken(string audience)
        {
            if (String.IsNullOrWhiteSpace(audience))
            {
                throw new ArgumentNullException(nameof(audience));
            }

            if (!Uri.IsWellFormedUriString(audience, UriKind.Absolute))
            {
                throw new ArgumentException(nameof(audience), "Audience should be an absolute URL");
            }

            Dictionary<string, object> jwtBody = GetJwtBody(audience);

            return GenerateJwtToken(_jwtHeader, jwtBody);
        }

        private Dictionary<string, object> GetJwtBody(string audience)
        {
            Dictionary<string, object> jwtBody = new Dictionary<string, object>
            {
                { AUDIENCE_CLAIM, audience },
                { EXPIRATION_CLAIM, GetAbsoluteExpiration(_expiration) }
            };

            if (_subject != null)
            {
                jwtBody.Add(SUBJECT_CLAIM, _subject);
            }

            return jwtBody;
        }

        private static long GetAbsoluteExpiration(int expirationSeconds)
        {
            TimeSpan unixEpochOffset = DateTime.UtcNow - _unixEpoch;

            return (long)unixEpochOffset.TotalSeconds + expirationSeconds;
        }

        private string GenerateJwtToken(Dictionary<string, string> jwtHeader, Dictionary<string, object> jwtBody)
        {
            string jwtInput = UrlBase64Converter.ToUrlBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jwtHeader)))
                + JWT_SEPARATOR
                + UrlBase64Converter.ToUrlBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jwtBody)));

            byte[] jwtInputHash;
            using (var sha256Hasher = SHA256.Create())
            {
                jwtInputHash = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(jwtInput));
            }

            ECDsaSigner jwtSigner = new ECDsaSigner();
            jwtSigner.Init(true, _privateSigningKey);

            BigInteger[] jwtSignature = jwtSigner.GenerateSignature(jwtInputHash);

            byte[] jwtSignatureFirstSegment = jwtSignature[0].ToByteArrayUnsigned();
            byte[] jwtSignatureSecondSegment = jwtSignature[1].ToByteArrayUnsigned();

            int jwtSignatureSegmentLength = Math.Max(jwtSignatureFirstSegment.Length, jwtSignatureSecondSegment.Length);
            byte[] combinedJwtSignature = new byte[2 * jwtSignatureSegmentLength];
            ByteArrayCopyWithPadLeft(jwtSignatureFirstSegment, combinedJwtSignature, 0, jwtSignatureSegmentLength);
            ByteArrayCopyWithPadLeft(jwtSignatureSecondSegment, combinedJwtSignature, jwtSignatureSegmentLength, jwtSignatureSegmentLength);

            return jwtInput + JWT_SEPARATOR + UrlBase64Converter.ToUrlBase64String(combinedJwtSignature);
        }

        private static void ByteArrayCopyWithPadLeft(byte[] sourceArray, byte[] destinationArray, int destinationIndex, int destinationLengthToUse)
        {
            if (sourceArray.Length != destinationLengthToUse)
            {
                destinationIndex += (destinationLengthToUse - sourceArray.Length);
            }

            Array.Copy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
        }
        #endregion
    }
}
