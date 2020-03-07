using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;

namespace Test.Lib.Net.Http.WebPush.Functional.Infrastructure
{
    public class FakePushServiceStartup
    {
        #region Fields
        public const int OTHER_CLIENT_ERROR_STATUS_CODE = 499;
        public const string OTHER_CLIENT_ERROR_REASON_PHRASE = "Other Client Error";
        public const string OTHER_CLIENT_ERROR_BODY = "{\"code\": 499,\"errno\": 199,\"error\": \"Other Client Error\",\"message\": \"Some other client error occured\"}";

        private bool shouldRetryAfter = true;
        #endregion

        public void ConfigureServices(IServiceCollection services)
        { }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/push-created", context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status201Created;

                        return Task.CompletedTask;
                    });

                    endpoints.MapPost("/push-retry-after", context =>
                    {
                        if (shouldRetryAfter)
                        {
                            context.Response.StatusCode = 429;
                            context.Response.Headers.Add("Retry-After", "5");

                            shouldRetryAfter = false;
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status201Created;

                            shouldRetryAfter = true;
                        }

                        return Task.CompletedTask;
                    });

                    endpoints.MapPost("/push-client-error", async context =>
                    {
                        context.Response.StatusCode = OTHER_CLIENT_ERROR_STATUS_CODE;
                        context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = OTHER_CLIENT_ERROR_REASON_PHRASE;
                        await context.Response.WriteAsync(OTHER_CLIENT_ERROR_BODY);
                    });
                });
        }
    }
}
