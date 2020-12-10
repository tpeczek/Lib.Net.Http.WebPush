using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Lib.Net.Http.WebPush;
using Lib.AspNetCore.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Lib.AspNetCore.WebPush.Caching;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions methods to configure an <see cref="IServiceCollection"/> for <see cref="PushServiceClient"/>.
    /// </summary>
    public static class PushServiceClientServiceCollectionExtensions
    {
        private const string HTTP_CLIENT_NAME = "Lib.AspNetCore.WebPush";

        private class VapidAuthenticationProvider : IDisposable
        {
            public VapidAuthentication VapidAuthentication { get; }

            public VapidAuthenticationProvider(VapidAuthentication vapidAuthentication)
            {
                VapidAuthentication = vapidAuthentication;
            }

            public void Dispose()
            {
                VapidAuthentication?.Dispose();
            }
        }

        /// <summary>
        /// Adds the <see cref="IMemoryCache"/> based implementation of <see cref="IVapidTokenCache"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddMemoryVapidTokenCache(this IServiceCollection services)
        {
            return services.AddSingleton<IVapidTokenCache, MemoryVapidTokenCache>();
        }

        /// <summary>
        /// Adds the <see cref="IDistributedCache"/> based implementation of <see cref="IVapidTokenCache"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDistributedVapidTokenCache(this IServiceCollection services)
        {
            return services.AddSingleton<IVapidTokenCache, DistributedVapidTokenCache>();
        }

        /// <summary>
        /// Adds the <see cref="PushServiceClient"/> and related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddPushServiceClient(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpClient(HTTP_CLIENT_NAME);

            services.AddSingleton(serviceProvider =>
            {
                VapidAuthentication vapidAuthentication = null;

                IOptions<PushServiceClientOptions> options = serviceProvider.GetRequiredService<IOptions<PushServiceClientOptions>>();
                if (options.Value != null)
                {
                    PushServiceClientOptions optionsValue = options.Value;

                    if (!String.IsNullOrWhiteSpace(optionsValue.PrivateKey) && !String.IsNullOrWhiteSpace(optionsValue.PublicKey))
                    {
                        vapidAuthentication = new VapidAuthentication(optionsValue.PublicKey, optionsValue.PrivateKey)
                        {
                            Subject = optionsValue.Subject
                        };

                        if (optionsValue.Expiration.HasValue)
                        {
                            vapidAuthentication.Expiration = optionsValue.Expiration.Value;
                        }

                        vapidAuthentication.TokenCache = serviceProvider.GetService<IVapidTokenCache>();
                    }
                }

                return new VapidAuthenticationProvider(vapidAuthentication);
            });

            services.AddTransient(serviceProvider =>
            {
                IHttpClientFactory clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                IOptions<PushServiceClientOptions> options = serviceProvider.GetRequiredService<IOptions<PushServiceClientOptions>>();

                PushServiceClient pushServiceClient = new PushServiceClient(clientFactory.CreateClient(HTTP_CLIENT_NAME));
                if (options.Value != null)
                {
                    PushServiceClientOptions optionsValue = options.Value;

                    VapidAuthenticationProvider vapidAuthenticationProvider = serviceProvider.GetRequiredService<VapidAuthenticationProvider>();
                    if (vapidAuthenticationProvider.VapidAuthentication != null)
                    {
                        pushServiceClient.DefaultAuthentication = vapidAuthenticationProvider.VapidAuthentication;
                    }

                    pushServiceClient.DefaultAuthenticationScheme = optionsValue.DefaultAuthenticationScheme;

                    if (optionsValue.DefaultTimeToLive.HasValue)
                    {
                        pushServiceClient.DefaultTimeToLive = optionsValue.DefaultTimeToLive.Value;
                    }

                    pushServiceClient.AutoRetryAfter = optionsValue.AutoRetryAfter;
                    pushServiceClient.MaxRetriesAfter = optionsValue.MaxRetriesAfter;
                }

                return pushServiceClient;
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="PushServiceClient"/> and related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureOptions">Used to configure the <see cref="PushServiceClientOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddPushServiceClient(this IServiceCollection services, Action<PushServiceClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddPushServiceClient();
            services.Configure(configureOptions);

            return services;
        }
    }
}
