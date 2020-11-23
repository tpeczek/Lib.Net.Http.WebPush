using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DocFx.Net.Http.WebPush
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        { }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles();
        }
    }
}
