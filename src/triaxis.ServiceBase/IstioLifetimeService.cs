using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace triaxis.ServiceBase
{
    internal class IstioLifetimeService : IHostedService
    {
        HttpClient _envoyClient;
        TimeSpan _startupTimeout;
        ILogger _logger;
        bool _istioRunning;

        public IstioLifetimeService(IOptions<EnvoyApiOptions> envoyApiOptions, ILogger<IstioLifetimeService> logger)
        {
            var opts = envoyApiOptions.Value;
            _envoyClient = new HttpClient()
            {
                BaseAddress = opts.EnvoyApiEndpoint,
            };
            _startupTimeout = opts.StartupTimeout;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var ctsTimeout = new CancellationTokenSource(_startupTimeout);
            using var ctsCombined = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeout.Token, cancellationToken);
            cancellationToken = ctsCombined.Token;

            _logger.LogInformation("Waiting for Istio sidecar...");
            for (;;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    using var response = await _envoyClient.GetAsync("ready", cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                }
                catch (HttpRequestException)
                {
                    // any error means istio is not ready yet
                }

                await Task.Delay(500, cancellationToken);
            }
            _logger.LogInformation("Istio sidecar ready");
            _istioRunning = true;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_istioRunning)
            {
                _logger.LogInformation("Stopping Istio sidecar...");
                await _envoyClient.PostAsync("quitquitquit", null, cancellationToken);
                _logger.LogInformation("Istio sidecar stopped");
                _istioRunning = false;
            }
        }
    }
}
