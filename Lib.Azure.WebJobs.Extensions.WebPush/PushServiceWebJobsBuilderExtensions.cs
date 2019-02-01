using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lib.Azure.WebJobs.Extensions.WebPush.Config;

namespace Lib.Azure.WebJobs.Extensions.WebPush
{
    /// <summary>
    /// The <see cref="IWebJobsBuilder"/> extension methods for Push Service binding extensions.
    /// </summary>
    public static class PushServiceWebJobsBuilderExtensions
    {
        /// <summary>
        /// Adds the Push Service binding extensions to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        /// <param name="configure">An <see cref="Action{PushServiceOptions}"/> to configure the provided <see cref="PushServiceOptions"/>.</param>
        public static IWebJobsBuilder AddPushService(this IWebJobsBuilder builder, Action<PushServiceOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddPushService();
            builder.Services.Configure(configure);

            return builder;
        }

        /// <summary>
        /// Adds the Push Service binding extensions to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        public static IWebJobsBuilder AddPushService(this IWebJobsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<PushServiceExtensionConfigProvider>()
                .ConfigureOptions<PushServiceOptions>((config, path, options) =>
                {
                    config.GetSection(path).Bind(options);
                });

            builder.Services.AddHttpClient();
            builder.Services.Configure<HttpClientFactoryOptions>(options => options.SuppressHandlerScope = true);

            return builder;
        }
    }
}
