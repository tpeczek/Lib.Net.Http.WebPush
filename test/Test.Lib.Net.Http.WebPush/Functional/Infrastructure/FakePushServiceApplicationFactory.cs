using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test.Lib.Net.Http.WebPush.Functional.Infrastructure
{
    public class FakePushServiceApplicationFactory : WebApplicationFactory<FakePushServiceStartup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<FakePushServiceStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Path.GetTempPath());
        }
    }
}
