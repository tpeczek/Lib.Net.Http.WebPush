using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Lib.Net.Http.WebPush.Functional.Infrastructure
{
    public class FakePushServiceStartup
    {
        private bool shouldRetryAfter = true;

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
                });
        }
    }
}
