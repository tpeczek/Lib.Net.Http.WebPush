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
		private const string CREATED_ENDPOINT = "http://localhost/push-created";
		private const string RETRY_AFTER_ENDPOINT = "http://localhost/push-retry-after";
		private const string WALRUS_CONTENT = "I am the walrus";

		private readonly PushSubscription _pushSubscription = new PushSubscription
		{
			Keys = new Dictionary<string, string>
				{
					{ "auth", "n5mG_PyMSKALsjsU542E6g" },
					{ "p256dh", "BDS52l6tfaf6ZEqhyDa0cScvCi4WXNYPIwmfas-7nKLIQex-DVKXB9gUxDExaZEOiwovl6LbWXZBZ9AT-GWT6eQ" }
				}
		};

		private readonly VapidAuthentication _vapidAuthentication = new VapidAuthentication("BK5sn4jfa0Jqo9MhV01oyzK2FaEHm0KqkSCuUkKr53-9cr-vBE1a9TiiBaWy7hy0eOUF1jhZnwcd3vof4wnwSw0", "AJ2ho7or-6D4StPktpTO3l1ErjHGyxb0jzt9Y8lj67g")
		{
			Subject = "https://localhost:8080/"
		};

		private readonly FakePushServiceApplicationFactory _pushServiceFactory;

        public PushMessageDeliveryTests(FakePushServiceApplicationFactory pushServiceFactory)
        {
			_pushServiceFactory = pushServiceFactory;
        }

		[Fact]
		public async Task DeliversPushMessage()
		{
			_pushSubscription.Endpoint = CREATED_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = new PushServiceClient(_pushServiceFactory.CreateClient());
			pushClient.DefaultAuthentication = _vapidAuthentication;

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}

		[Fact]
		public async Task DeliversPushMessageWithRetryAfter()
		{
			_pushSubscription.Endpoint = RETRY_AFTER_ENDPOINT;

			PushMessage pushMessage = new PushMessage(WALRUS_CONTENT);

			PushServiceClient pushClient = new PushServiceClient(_pushServiceFactory.CreateClient());
			pushClient.DefaultAuthentication = _vapidAuthentication;

			Exception pushMessageDeliveryException = await Record.ExceptionAsync(async () =>
			{
				await pushClient.RequestPushMessageDeliveryAsync(_pushSubscription, pushMessage);
			});

			Assert.Null(pushMessageDeliveryException);
		}
	}
}
