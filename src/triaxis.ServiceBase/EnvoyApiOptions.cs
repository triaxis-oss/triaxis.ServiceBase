using System;

namespace triaxis.ServiceBase
{
    /// <summary>
    /// Options for accessing the Envoy sidecar API during startup and shutdown
    /// </summary>
    public class EnvoyApiOptions
    {
        /// <summary>
        /// Url of the Envoy sidecar API, for Istio this is typically
        /// http://localhost:15000
        /// </summary>
        public Uri EnvoyApiEndpoint { get; set; }
        /// <summary>
        /// Timeout for sidecar startup wait
        /// </summary>
        public TimeSpan StartupTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
