using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using static Lib.Net.Http.WebPush.Authentication.VapidAuthentication;

namespace Benchmark.Net.Http.WebPush
{
    [MemoryDiagnoser]
    public class WebPushBenchmarks
    {
        #region Classes
        private class RequestPushMessageDeliveryVapidTokenCache : IVapidTokenCache
        {
            public string Get(string audience)
            {
                return "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NiJ9.eyJhdWQiOiJodHRwczovL2ZjbS5nb29nbGVhcGlzLmNvbSIsImV4cCI6MTUxODMzMzkyNCwic3ViIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NjU1MDYvIn0.RP8t19y_3c6ncoE7iHOukEEKxIb8nBHdOaeY8xzLoMw62-GlWR5C1Rp8iG2rex9_pk_1LR4MJSAkMpDbhnZo5w";
            }

            public void Put(string audience, DateTimeOffset expiration, string token)
            { }
        }

        private class RequestPushMessageDeliveryHttpMessageHandler : HttpMessageHandler
        {
            Task<HttpResponseMessage> _createdResponseMessageTask = Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _createdResponseMessageTask;
            }
        }
        #endregion

        #region Fields
        private readonly PushMessage _message = new PushMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet dolor tristique, tempus mi ultrices, vulputate erat. Nulla viverra mauris et ante pharetra accumsan. Praesent venenatis nibh vel aliquam pharetra. Suspendisse massa justo, mollis vitae venenatis eu, mattis sit amet ipsum. Etiam dolor lacus, vulputate id congue eu, mattis vel orci. Curabitur dignissim posuere vehicula. Quisque tristique tellus ligula, sit amet tincidunt sapien dignissim ut. Quisque pulvinar justo non turpis vehicula, hendrerit semper mi gravida. In vitae massa et erat commodo dignissim at id massa. Etiam non fringilla dolor, eu sagittis elit. Vestibulum finibus molestie viverra. Duis urna libero, pulvinar et mollis sed, volutpat sed orci. Sed dapibus vitae urna a volutpat. Phasellus ultricies eget quam commodo sagittis. Nunc sollicitudin ullamcorper faucibus. Vestibulum tempus molestie justo, at tristique urna finibus vel. Nam vitae eros gravida, tincidunt lorem quis, lacinia dui. Aliquam pretium metus nec risus scelerisque, eget auctor quam maximus. Vestibulum tempor metus egestas, maximus felis id, cursus diam. Sed non libero quis nibh scelerisque pellentesque vel nec nunc. Aliquam luctus ornare justo, at fermentum orci scelerisque at. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Etiam tempus finibus sem ac egestas. Fusce vitae justo ligula. Integer vel felis maximus odio commodo auctor eu quis nisl. Interdum et malesuada fames ac ante ipsum primis in faucibus. Suspendisse finibus mauris mattis luctus luctus. Integer imperdiet turpis vitae elit interdum sagittis. Aenean semper at sapien sit amet porttitor. Proin vel lacus vestibulum, consectetur urna sed, ultricies eros. Integer feugiat enim quis posuere ultricies. Quisque ac mattis diam. Nulla non blandit neque. Vivamus non elementum leo, eget vestibulum odio. Curabitur dignissim justo urna, quis imperdiet metus porttitor a. Nunc porta justo erat, vel malesuada mauris consectetur sed. Etiam fermentum dapibus mi, a ultrices ex euismod ut. Vivamus fringilla, sem ac pulvinar auctor, arcu mauris viverra erat, nec tempor orci nunc nec ipsum. Donec hendrerit eros aliquam finibus efficitur. Aliquam id sem eu urna volutpat consequat. Integer varius tellus eget sem cursus laoreet. Suspendisse potenti. Duis a dolor eros. Proin aliquet nisl dui, at faucibus odio porttitor a. Ut at fermentum purus, vel tincidunt massa. Aliquam ornare ornare augue ut ullamcorper. In cursus auctor purus, sodales suscipit enim sagittis vel. Etiam nec semper magna. Vivamus a mi congue, efficitur nisl vitae, sagittis urna. Vivamus ut ex a enim commodo feugiat. Nam dictum egestas neque eget tristique. Nam interdum mi non ornare rhoncus. Nam mi arcu, placerat sollicitudin ligula eu, efficitur hendrerit elit. Aliquam suscipit lacinia ante ac suscipit. Aliquam malesuada dolor at lorem mattis, ut feugiat mauris semper. Nulla tortor leo, porttitor eu ipsum vel, lobortis bibendum tortor. Quisque semper quam at cursus laoreet. Fusce in facilisis orci. Vivamus dapibus ac arcu ut condimentum. Morbi mollis a turpis nec sollicitudin. Aliquam fringilla massa elit, a mollis velit rhoncus eget. Aliquam erat volutpat. Vestibulum ante mauris, aliquet non purus ac, ornare pretium neque. Maecenas nec tortor justo. Curabitur ullamcorper placerat mauris vel scelerisque. Quisque maximus nunc sit amet aliquam pulvinar. Maecenas ullamcorper dictum efficitur. Proin auctor, sapien nec efficitur semper, dolor purus faucibus ipsum, fringilla consequat ipsum magna quis dui. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam erat volutpat. Suspendisse potenti. Maecenas orci odio, interdum ut lacus ac, sagittis molestie turpis. Donec pretium diam quis ex aliquam, quis rutrum ipsum egestas. Maecenas eu leo finibus, tincidunt leo sed, accumsan tellus. Sed aliquam placerat lectus, at tempor massa volutpat id. Integer a viverra ipsum. Etiam eu gravida risus. Maecenas turpis, porta vitae dignissim a, facilisis ac tortor. Nulla nec sed.");
        private readonly PushSubscription _subscription = new PushSubscription
        {
            Endpoint = "https://fcm.googleapis.com/fcm/send/fvi1URmMPCw:APA91bGqK_mH7LfRzY1wKeTj_kpealAK_HhRKpTNPSQ7dW8TQjR4qK4a7BcZcMRIHU9A6cH6iimqP7zyirmxjKd4vOKwos_itEI78bVF0LqCn5Xv99QkMxW_YZR7d2j993LapxJvzZ_V",
            Keys = new Dictionary<string, string>
            {
                { "p256dh", "BP7M3CU6iBsNCJZAYk2akn2j236QjtxXZ6hMZP9H6XKTHFmjG1Al-fVW49Dz8Zlr_Qtqz30fv3yNrRelpj8Gvtg=" },
                { "auth", "YE41bJSLtSq-hraJ17ryyA==" }
            }
        };

        private VapidAuthentication _vapidAuthentication = new VapidAuthentication("BK5sn4jfa0Jqo9MhV01oyzK2FaEHm0KqkSCuUkKr53-9cr-vBE1a9TiiBaWy7hy0eOUF1jhZnwcd3vof4wnwSw0", "AJ2ho7or-6D4StPktpTO3l1ErjHGyxb0jzt9Y8lj67g")
        {
            Subject = "https://localhost:65506/"
        };

        private PushServiceClient _pushClient = new PushServiceClient(new HttpClient(new RequestPushMessageDeliveryHttpMessageHandler()))
        {
            DefaultAuthentication = new VapidAuthentication("BK5sn4jfa0Jqo9MhV01oyzK2FaEHm0KqkSCuUkKr53-9cr-vBE1a9TiiBaWy7hy0eOUF1jhZnwcd3vof4wnwSw0", "AJ2ho7or-6D4StPktpTO3l1ErjHGyxb0jzt9Y8lj67g")
            {
                Subject = "https://localhost:65506/",
                TokenCache = new RequestPushMessageDeliveryVapidTokenCache()
            }
        };
        #endregion

        #region Benchmarks
        [Benchmark]
        public WebPushSchemeHeadersValues VapidAuthentication_GetVapidSchemeAuthenticationHeaderValueParameter()
        {
            return _vapidAuthentication.GetWebPushSchemeHeadersValues("https://fcm.googleapis.com");
        }

        [Benchmark]
        public Task PushServiceClient_RequestPushMessageDeliveryAsync()
        {
            return _pushClient.RequestPushMessageDeliveryAsync(_subscription, _message);
        }
        #endregion
    }
}
