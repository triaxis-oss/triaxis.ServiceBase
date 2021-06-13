using System;
using Microsoft.Extensions.DependencyInjection;
using triaxis.ServiceBase;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for configuring an <see cref="IHostBuilder" />
    /// </summary>
    public static class HostingExtensions
    {
        /// <summary>
        /// Registers an <see cref="IHostedService" /> that waits for the Envoy sidecar
        /// on startup and terminates it on shutdown.
        /// An environment variable <c>ISTIO_ENVOY_API</c> must be defined for the
        /// service to be registered.
        /// This method should be called immediately after <c>Host.CreateDefaultBuilder</c>
        /// to avoid startup order issues.
        /// </summary>
        public static IHostBuilder UseIstioSidecar(this IHostBuilder builder)
        {
            if (Uri.TryCreate(Environment.GetEnvironmentVariable("ISTIO_ENVOY_API"), UriKind.Absolute, out var envoyUri))
            {
                builder.ConfigureServices((context, services) =>
                {
                    services.AddOptions<EnvoyApiOptions>().Configure(opts => opts.EnvoyApiEndpoint = envoyUri);
                    services.AddHostedService<IstioLifetimeService>();
                });
            }

            return builder;
        }
    }
}
