using System;
using System.Text;
using System.Globalization;
using Lib.Net.Http.WebPush.Internals;

namespace Lib.Net.Http.WebPush.Authentication
{
    /// <summary>
    /// Class which provides Voluntary Application Server Identification (VAPID) headers values.
    /// </summary>
    public class VapidAuthentication : IDisposable
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

        private const int DEFAULT_EXPIRATION = 43200;
        private const int MAXIMUM_EXPIRATION = 86400;

        private const string JWT_HEADER = "{\"typ\":\"JWT\",\"alg\":\"ES256\"}";
        private const string JWT_SEPARATOR = ".";
        private const string JWT_BODY_AUDIENCE_PART = "{\"aud\":\"";
        private const string JWT_BODY_EXPIRATION_PART = "\",\"exp\":";
        private const string JWT_BODY_SUBJECT_PART = ",\"sub\":\"";
        private const string JWT_BODY_WITH_SUBJECT_CLOSING = "\"}";
        private const string JWT_BODY_WITHOUT_SUBJECT_CLOSING = "}";

        private const string P256ECDSA_PREFIX = "p256ecdsa=";
        private const string VAPID_AUTHENTICATION_HEADER_VALUE_PARAMETER_FORMAT = "t={0}, k={1}";

        private string _subject;
        private string _publicKey;
        private string _privateKey;
        private int _relativeExpiration;

        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private static readonly string _jwtHeaderSegment = UrlBase64Converter.ToUrlBase64String(Encoding.UTF8.GetBytes(JWT_HEADER));

        private ES256Signer _jwtSigner;
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

                _jwtSigner = new ES256Signer(decodedPrivateKey);
            }
        }

        /// <summary>
        /// Gets or sets the time after which the authentication token expires (in seconds).
        /// </summary>
        public int Expiration
        {
            get { return _relativeExpiration; }

            set
            {
                if ((value <= 0) || (value > MAXIMUM_EXPIRATION))
                {
                    throw new ArgumentOutOfRangeException(nameof(Expiration), "Expiration must be a number of seconds not longer than 24 hours");
                }

                _relativeExpiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the token cache.
        /// </summary>
        public IVapidTokenCache TokenCache { get; set; }
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

            _relativeExpiration = DEFAULT_EXPIRATION;
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

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="VapidAuthentication"/> class.
        /// </summary>
        public void Dispose()
        {
            _jwtSigner?.Dispose();
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

            string token = TokenCache?.Get(audience);

            if (token == null)
            {
                DateTime absoluteExpiration = DateTime.UtcNow.AddSeconds(_relativeExpiration);

                token = GenerateToken(audience, absoluteExpiration);

                TokenCache?.Put(audience, absoluteExpiration, token);
            }

            return token;
        }

        private string GenerateToken(string audience, DateTime absoluteExpiration)
        {
            string jwtInput = _jwtHeaderSegment + JWT_SEPARATOR + GenerateJwtBodySegment(audience, absoluteExpiration);

            return jwtInput + JWT_SEPARATOR + UrlBase64Converter.ToUrlBase64String(_jwtSigner.GenerateSignature(jwtInput));
        }

        private string GenerateJwtBodySegment(string audience, DateTime absoluteExpiration)
        {
            StringBuilder jwtBodyBuilder = new StringBuilder();

            jwtBodyBuilder.Append(JWT_BODY_AUDIENCE_PART).Append(audience)
                .Append(JWT_BODY_EXPIRATION_PART).Append(ToUnixTimeSeconds(absoluteExpiration).ToString(CultureInfo.InvariantCulture));

            if (_subject != null)
            {
                jwtBodyBuilder.Append(JWT_BODY_SUBJECT_PART).Append(_subject).Append(JWT_BODY_WITH_SUBJECT_CLOSING);
            }
            else
            {
                jwtBodyBuilder.Append(JWT_BODY_WITHOUT_SUBJECT_CLOSING);
            }

            return UrlBase64Converter.ToUrlBase64String(Encoding.UTF8.GetBytes(jwtBodyBuilder.ToString()));
        }

        private static long ToUnixTimeSeconds(DateTime dateTime)
        {
            TimeSpan unixEpochOffset = dateTime - _unixEpoch;

            return (long)unixEpochOffset.TotalSeconds;
        }
        #endregion
    }
}
