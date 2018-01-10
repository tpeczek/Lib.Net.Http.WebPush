using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Lib.Net.Http.EncryptedContentEncoding;
using Lib.Net.Http.WebPush.Internals;
using Lib.Net.Http.WebPush.Authentication;

namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// A Web Push Protocol compliant client for push service.
    /// </summary>
    /// <remarks>
    /// The <see cref="PushServiceClient"/> should be considered an expensive object as it internally holds an instance of <see cref="HttpClient"/> class. In order to avoid Improper Instantiation antipattern a shared singleton instance should be created or a pool of reusable instances should be used.
    /// </remarks>
    public class PushServiceClient
    {
        #region Fields
        private const string TTL_HEADER_NAME = "TTL";
        private const string URGENCY_HEADER_NAME = "Urgency";
        private const string CRYPTO_KEY_HEADER_NAME = "Crypto-Key";
        private const string WEBPUSH_AUTHENTICATION_SCHEME = "WebPush";
        private const string VAPID_AUTHENTICATION_SCHEME = "vapid";

        private const int DEFAULT_TIME_TO_LIVE = 2419200;

        private const string KEYING_MATERIAL_INFO_PARAMETER_PREFIX = "WebPush: info";
        private const byte KEYING_MATERIAL_INFO_PARAMETER_DELIMITER = 1;
        private const int KEYING_MATERIAL_INFO_PARAMETER_LENGTH = 32;

        private const int CONTENT_RECORD_SIZE = 4096;

        private static readonly byte[] _keyingMaterialInfoParameterPrefix = Encoding.ASCII.GetBytes(KEYING_MATERIAL_INFO_PARAMETER_PREFIX);

        private int _defaultTimeToLive = DEFAULT_TIME_TO_LIVE;

        private readonly HttpClient _httpClient = new HttpClient();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default time (in seconds) for which the message should be retained by push service. It will be used when <see cref="PushMessage.TimeToLive"/> is not set.
        /// </summary>
        public int DefaultTimeToLive
        {
            get { return _defaultTimeToLive; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(DefaultTimeToLive), "The TTL must be a non-negative integer");
                }

                _defaultTimeToLive = value;
            }
        }

        /// <summary>
        /// Gets or sets the default authentication details.
        /// </summary>
        public VapidAuthentication DefaultAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="VapidAuthenticationScheme"/> to be used.
        /// </summary>
        public VapidAuthenticationScheme DefaultAuthenticationScheme { get; set; } = VapidAuthenticationScheme.WebPush;
        #endregion

        #region Methods
        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message)
        {
            return RequestPushMessageDeliveryAsync(subscription, message, null, DefaultAuthenticationScheme, CancellationToken.None);
        }

        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken)
        {
            return RequestPushMessageDeliveryAsync(subscription, message, null, DefaultAuthenticationScheme, cancellationToken);
        }

        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <param name="authentication">The authentication details.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message, VapidAuthentication authentication)
        {
            return RequestPushMessageDeliveryAsync(subscription, message, authentication, DefaultAuthenticationScheme, CancellationToken.None);
        }

        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <param name="authentication">The authentication details.</param>
        /// <param name="authenticationScheme">The <see cref="VapidAuthenticationScheme"/> to use.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message, VapidAuthentication authentication, VapidAuthenticationScheme authenticationScheme)
        {
            return RequestPushMessageDeliveryAsync(subscription, message, authentication, authenticationScheme, CancellationToken.None);
        }

        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <param name="authentication">The authentication details.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message, VapidAuthentication authentication, CancellationToken cancellationToken)
        {
            return RequestPushMessageDeliveryAsync(subscription, message, authentication, DefaultAuthenticationScheme, cancellationToken);
        }

        /// <summary>
        /// Requests delivery of push message by push service as an asynchronous operation.
        /// </summary>
        /// <param name="subscription">The push service subscription.</param>
        /// <param name="message">The push message.</param>
        /// <param name="authentication">The authentication details.</param>
        /// <param name="authenticationScheme">The <see cref="VapidAuthenticationScheme"/> to use.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task RequestPushMessageDeliveryAsync(PushSubscription subscription, PushMessage message, VapidAuthentication authentication, VapidAuthenticationScheme authenticationScheme, CancellationToken cancellationToken)
        {
            HttpRequestMessage pushMessageDeliveryRequest = PreparePushMessageDeliveryRequest(subscription, message, authentication, authenticationScheme);

            HttpResponseMessage pushMessageDeliveryRequestResponse = await _httpClient.SendAsync(pushMessageDeliveryRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            HandlePushMessageDeliveryRequestResponse(pushMessageDeliveryRequestResponse);
        }

        private HttpRequestMessage PreparePushMessageDeliveryRequest(PushSubscription subscription, PushMessage message, VapidAuthentication authentication, VapidAuthenticationScheme authenticationScheme)
        {
            authentication = authentication ?? DefaultAuthentication;
            if (authentication == null)
            {
                throw new InvalidOperationException("The VAPID authentication information is not available");
            }

            HttpRequestMessage pushMessageDeliveryRequest = new HttpRequestMessage(HttpMethod.Post, subscription.Endpoint)
            {
                Headers =
                {
                    { TTL_HEADER_NAME, (message.TimeToLive ?? DefaultTimeToLive).ToString(CultureInfo.InvariantCulture) }
                }
            };
            pushMessageDeliveryRequest = SetAuthentication(pushMessageDeliveryRequest, subscription, authentication, authenticationScheme);
            pushMessageDeliveryRequest = SetContent(pushMessageDeliveryRequest, subscription, message);

            return pushMessageDeliveryRequest;
        }

        private static HttpRequestMessage SetAuthentication(HttpRequestMessage pushMessageDeliveryRequest, PushSubscription subscription, VapidAuthentication authentication, VapidAuthenticationScheme authenticationScheme)
        {
            Uri endpointUri = new Uri(subscription.Endpoint);
            string audience = endpointUri.Scheme + @"://" + endpointUri.Host;

            if (authenticationScheme == VapidAuthenticationScheme.WebPush)
            {
                VapidAuthentication.WebPushSchemeHeadersValues webPushSchemeHeadersValues = authentication.GetWebPushSchemeHeadersValues(audience);

                pushMessageDeliveryRequest.Headers.Authorization = new AuthenticationHeaderValue(WEBPUSH_AUTHENTICATION_SCHEME, webPushSchemeHeadersValues.AuthenticationHeaderValueParameter);
                pushMessageDeliveryRequest.Headers.Add(CRYPTO_KEY_HEADER_NAME, webPushSchemeHeadersValues.CryptoKeyHeaderValue);
            }
            else
            {
                pushMessageDeliveryRequest.Headers.Authorization = new AuthenticationHeaderValue(VAPID_AUTHENTICATION_SCHEME, authentication.GetVapidSchemeAuthenticationHeaderValueParameter(audience));
            }

            return pushMessageDeliveryRequest;
        }

        private static HttpRequestMessage SetContent(HttpRequestMessage pushMessageDeliveryRequest, PushSubscription subscription, PushMessage message)
        {
            if (String.IsNullOrEmpty(message.Content))
            {
                pushMessageDeliveryRequest.Content = null;
            }
            else
            {
                AsymmetricCipherKeyPair applicationServerKeys = ECKeyHelper.GenerateAsymmetricCipherKeyPair();
                byte[] applicationServerPublicKey = ((ECPublicKeyParameters)applicationServerKeys.Public).Q.GetEncoded(false);

                pushMessageDeliveryRequest.Content = new Aes128GcmEncodedContent(
                    new StringContent(message.Content, Encoding.UTF8),
                    GetKeyingMaterial(subscription, applicationServerKeys.Private, applicationServerPublicKey),
                    applicationServerPublicKey,
                    CONTENT_RECORD_SIZE
                );
            }

            return pushMessageDeliveryRequest;
        }

        private static byte[] GetKeyingMaterial(PushSubscription subscription, AsymmetricKeyParameter applicationServerPrivateKey, byte[] applicationServerPublicKey)
        {
            IBasicAgreement ecdhAgreement = AgreementUtilities.GetBasicAgreement("ECDH");
            ecdhAgreement.Init(applicationServerPrivateKey);

            byte[] userAgentPublicKey = UrlBase64Converter.FromUrlBase64String(subscription.GetKey(PushEncryptionKeyName.P256DH));
            byte[] authenticationSecret = UrlBase64Converter.FromUrlBase64String(subscription.GetKey(PushEncryptionKeyName.Auth));
            byte[] sharedSecret = ecdhAgreement.CalculateAgreement(ECKeyHelper.GetECPublicKeyParameters(userAgentPublicKey)).ToByteArrayUnsigned();
            byte[] sharedSecretHash = HmacSha256(authenticationSecret, sharedSecret);
            byte[] infoParameter = GetKeyingMaterialInfoParameter(userAgentPublicKey, applicationServerPublicKey);

            byte[] keyingMaterial = HmacSha256(sharedSecretHash, infoParameter);
            Array.Resize(ref keyingMaterial, KEYING_MATERIAL_INFO_PARAMETER_LENGTH);

            return keyingMaterial;
        }

        private static byte[] GetKeyingMaterialInfoParameter(byte[] userAgentPublicKey, byte[] applicationServerPublicKey)
        {
            // "WebPush: info" || 0x00 || ua_public || as_public || 0x01
            byte[] infoParameter = new byte[_keyingMaterialInfoParameterPrefix.Length + userAgentPublicKey.Length + applicationServerPublicKey.Length + 2];

            Array.Copy(_keyingMaterialInfoParameterPrefix, infoParameter, _keyingMaterialInfoParameterPrefix.Length);
            int infoParameterIndex = _keyingMaterialInfoParameterPrefix.Length + 1;

            Array.Copy(userAgentPublicKey, 0, infoParameter, infoParameterIndex, userAgentPublicKey.Length);
            infoParameterIndex += userAgentPublicKey.Length;

            Array.Copy(applicationServerPublicKey, 0, infoParameter, infoParameterIndex, applicationServerPublicKey.Length);

            infoParameter[infoParameter.Length - 1] = KEYING_MATERIAL_INFO_PARAMETER_DELIMITER;

            return infoParameter;
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

        private static void HandlePushMessageDeliveryRequestResponse(HttpResponseMessage pushMessageDeliveryRequestResponse)
        {
            if (pushMessageDeliveryRequestResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new PushServiceClientException(pushMessageDeliveryRequestResponse.ReasonPhrase, pushMessageDeliveryRequestResponse.StatusCode);
            }
        }
        #endregion
    }
}
