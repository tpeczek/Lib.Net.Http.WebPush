using System.Collections.Generic;
using Xunit;
using Lib.Net.Http.WebPush;

namespace Test.Lib.Net.Http.WebPush.Unit
{
    public class PushSubscriptionTests
    {
		#region Fields
		private const string AUTH_KEY = "n5mG_PyMSKALsjsU542E6g";
		private const string P256DH_KEY = "BDS52l6tfaf6ZEqhyDa0cScvCi4WXNYPIwmfas-7nKLIQex-DVKXB9gUxDExaZEOiwovl6LbWXZBZ9AT-GWT6eQ";
		#endregion

		#region Tests
		[Fact]
		public void GetKey_AuthKeyNameLowercase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "auth", AUTH_KEY }
				}
			};

			string authKey = pushSubscription.GetKey(PushEncryptionKeyName.Auth);

			Assert.Equal(AUTH_KEY, authKey);
		}

		[Fact]
		public void GetKey_AuthKeyNameUppercase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "AUTH", AUTH_KEY }
				}
			};

			string authKey = pushSubscription.GetKey(PushEncryptionKeyName.Auth);

			Assert.Equal(AUTH_KEY, authKey);
		}

		[Fact]
		public void GetKey_AuthKeyNameMixedCase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "AuTh", AUTH_KEY }
				}
			};

			string authKey = pushSubscription.GetKey(PushEncryptionKeyName.Auth);

			Assert.Equal(AUTH_KEY, authKey);
		}

		[Fact]
		public void GetKey_P256DHKeyNameLowercase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "p256dh", P256DH_KEY }
				}
			};

			string p256dhKey = pushSubscription.GetKey(PushEncryptionKeyName.P256DH);

			Assert.Equal(P256DH_KEY, p256dhKey);
		}

		[Fact]
		public void GetKey_P256DHKeyNameUppercase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "P256DH", P256DH_KEY }
				}
			};

			string p256dhKey = pushSubscription.GetKey(PushEncryptionKeyName.P256DH);

			Assert.Equal(P256DH_KEY, p256dhKey);
		}

		[Fact]
		public void GetKey_P256DHKeyNameMixedCase_ReturnsKey()
		{
			PushSubscription pushSubscription = new PushSubscription
			{
				Keys = new Dictionary<string, string>
				{
					{ "P256dH", P256DH_KEY }
				}
			};

			string p256dhKey = pushSubscription.GetKey(PushEncryptionKeyName.P256DH);

			Assert.Equal(P256DH_KEY, p256dhKey);
		}
		#endregion
	}
}
