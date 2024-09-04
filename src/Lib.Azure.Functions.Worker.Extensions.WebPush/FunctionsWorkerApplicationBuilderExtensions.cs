using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Azure.Functions.Worker.Extensions.WebPush
{
    internal static class FunctionsWorkerApplicationBuilderExtensions
    {
        public static IFunctionsWorkerApplicationBuilder ConfigurePushServiceExtension(this IFunctionsWorkerApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            builder.Services.AddHttpClient();

            return builder;
        }
    }
}
