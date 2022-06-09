using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Test.Lib.Net.Http.WebPush.Functional.Infrastructure;

namespace Test.Lib.Net.Http.WebPush.Functional
{
    public class PushMessageDeliveryTests : IClassFixture<FakePushServiceApplicationFactory>
    {
        #region Fields
        private const string CREATED_ENDPOINT = "http://localhost/push-created";
		private const string RETRY_AFTER_ONCE_ENDPOINT = "http://localhost/push-retry-after-once";
		private const string RETRY_AFTER_ALWAYS_ENDPOINT = "http://localhost/push-retry-after-always";
		private const string CLIENT_ERROR_ENDPOINT = "http://localhost/push-client-error";

		private const string WALRUS_CONTENT = "I am the walrus";

		private const string PUSH_SUBSCRIPTION_AUTH_KEY = "n5mG_PyMSKALsjsU542E6g";
		private const string PUSH_SUBSCRIPTION_P256DH_KEY = "BDS52l6tfaf6ZEqhyDa0cScvCi4WXNYPIwmfas-7nKLIQex-DVKXB9gUxDExaZEOiwovl6LbWXZBZ9AT-GWT6eQ";

		private readonly PushSubscription _pushSubscription = new PushSubscription
		{
			Keys = new Dictionary<string, string>
				{
					{ "auth", PUSH_SUBSCRIPTION_AUTH_KEY },
					{ "p256dh", PUSH_SUBSCRIPTION_P256DH_KEY }
				}
		};

		private readonly VapidAuthentication _vapidAuthentication = new VapidAuthentication("BK5sn4jfa0Jqo9MhV01oyzK2FaEHm0KqkSCuUkKr53-9cr-vBE1a9TiiBaWy7hy0eOUF1jhZnwcd3vof4wnwSw0", "AJ2ho7or-6D4StPktpTO3l1ErjHGyxb0jzt9Y8lj67g")
		{
			Subject = "https://localhost:8080/"
		};

		private readonly FakePushServiceApplicationFactory _pushServiceFactory;
        #endregion

        #region Constructor
        public PushMessageDeliveryTests(FakePushServiceApplicationFactory pushServiceFactory)
        {
			_pushServiceFactory = pushServiceFactory;
        }
		#endregion

		#region Prepare SUT
		private PushServiceClient PreparePushServiceClient()
		{
			return new PushServiceClient(_pushServiceFactory.CreateClient())
			{
				DefaultAuthentication = _vapidAuthentication
			};
		}
		#endregion

		#region Tests
		[Fact]
		public async Task PushService_NoError_DeliversPushMessage()
		{
			_pushSubscription.Endpoint = CREATED_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}

		[Fact]
		public async Task PushService_TooManyRequests_DeliversPushMessageWithRetryAfter()
		{
			_pushSubscription.Endpoint = RETRY_AFTER_ONCE_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}

		[Fact]
		public async Task PushService_TooManyRequests_MaxRetriesAfter_ThrowsPushServiceClientException()
		{
			_pushSubscription.Endpoint = RETRY_AFTER_ALWAYS_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();
			pushClient.MaxRetriesAfter = 1;

			await Assert.ThrowsAsync<PushServiceClientException>(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});
		}

		[Fact]
		public async Task PushService_TooManyRequests_MaxRetriesAfter_PushServiceClientExceptionStatusCodeIs429()
		{
			_pushSubscription.Endpoint = RETRY_AFTER_ALWAYS_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();
			pushClient.MaxRetriesAfter = 1;

			PushServiceClientException pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			}) as PushServiceClientException;

			Assert.Equal(429, (int)pushMessageDeliveryException.StatusCode);
		}

		[Fact]
		public async Task PushService_OtherClientError_ThrowsPushServiceClientException()
		{
			_pushSubscription.Endpoint = CLIENT_ERROR_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			await Assert.ThrowsAsync<PushServiceClientException>(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});
		}

		[Fact]
		public async Task PushService_OtherClientError_PushServiceClientExceptionContainsResponseStatusCode()
		{
			_pushSubscription.Endpoint = CLIENT_ERROR_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			PushServiceClientException pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			}) as PushServiceClientException;

			Assert.Equal(FakePushServiceStartup.OTHER_CLIENT_ERROR_STATUS_CODE, (int)pushMessageDeliveryException.StatusCode);
		}

		[Fact]
		public async Task PushService_OtherClientError_PushServiceClientExceptionContainsResponseReasonPhrase()
		{
			_pushSubscription.Endpoint = CLIENT_ERROR_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			PushServiceClientException pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			}) as PushServiceClientException;

			Assert.Equal(FakePushServiceStartup.OTHER_CLIENT_ERROR_REASON_PHRASE, pushMessageDeliveryException.Message);
		}

		[Fact]
		public async Task PushService_OtherClientError_PushServiceClientExceptionContainsResponseBody()
		{
			_pushSubscription.Endpoint = CLIENT_ERROR_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			PushServiceClientException pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			}) as PushServiceClientException;

			Assert.Equal(FakePushServiceStartup.OTHER_CLIENT_ERROR_BODY, pushMessageDeliveryException.Body);
		}

		[Fact]
		public async Task PushService_OtherClientError_PushServiceClientExceptionContainsPushSubscription()
		{
			_pushSubscription.Endpoint = CLIENT_ERROR_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			PushServiceClientException pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			}) as PushServiceClientException;

			Assert.Equal(_pushSubscription, pushMessageDeliveryException.PushSubscription);
		}

		[Fact]
		public async Task PushService_PushEncryptionKeysNamesLowercase_DeliversPushMessage()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "auth", PUSH_SUBSCRIPTION_AUTH_KEY },
					{ "p256dh", PUSH_SUBSCRIPTION_P256DH_KEY }
				},
				Endpoint = CREATED_ENDPOINT
			};

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}

		[Fact]
		public async Task PushService_PushEncryptionKeysNamesUppercase_DeliversPushMessage()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "AUTH", PUSH_SUBSCRIPTION_AUTH_KEY },
					{ "P256DH", PUSH_SUBSCRIPTION_P256DH_KEY }
				},
				Endpoint = CREATED_ENDPOINT
			};

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}

		[Fact]
		public async Task PushService_PushEncryptionKeysNamesMixedCase_DeliversPushMessage()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "AuTh", PUSH_SUBSCRIPTION_AUTH_KEY },
					{ "P256dH", PUSH_SUBSCRIPTION_P256DH_KEY }
				},
				Endpoint = CREATED_ENDPOINT
			};

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = PreparePushServiceClient();

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}
		#endregion
	}
}
