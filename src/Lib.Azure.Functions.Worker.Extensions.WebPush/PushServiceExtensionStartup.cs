using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Core;

[assembly: WorkerExtensionStartup(typeof(Lib.Azure.Functions.Worker.Extensions.WebPush.PushServiceExtensionStartup))]

namespace Lib.Azure.Functions.Worker.Extensions.WebPush
{
    /// <summary>
    /// Class providing a worker extension startup implementation.
    /// </summary>
    public class PushServiceExtensionStartup : WorkerExtensionStartup
    {
        /// <summary>
        /// Performs the startup configuration action for Push Service extension.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IFunctionsWorkerApplicationBuilder"/> that can be used to configure the worker extension.</param>
        public override void Configure(IFunctionsWorkerApplicationBuilder applicationBuilder)
        {
            ArgumentNullException.ThrowIfNull(applicationBuilder, nameof(applicationBuilder));

            applicationBuilder.ConfigurePushServiceExtension();
        }
    }
}
