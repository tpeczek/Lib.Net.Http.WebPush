using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Lib.Azure.WebJobs.Extensions.WebPush;

[assembly: WebJobsStartup(typeof(PushServiceWebJobsStartup))]

namespace Lib.Azure.WebJobs.Extensions.WebPush
{
    /// <summary>
    /// Class defining a startup configuration action for Push Service binding extensions, which will be performed as part of host startup.
    /// </summary>
    public class PushServiceWebJobsStartup : IWebJobsStartup
    {
        /// <summary>
        /// Performs the startup configuration action for Push Service binding extensions.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> that can be used to configure the host.</param>
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddPushService();
        }
    }
}
