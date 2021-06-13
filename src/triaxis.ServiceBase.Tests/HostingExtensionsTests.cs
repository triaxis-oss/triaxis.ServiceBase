using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace triaxis.ServiceBase.Tests
{
    public class HostingExtensionsTests
    {
        IHostBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = Host.CreateDefaultBuilder();
        }

        [Test]
        public void UseIstioSidecar_EnvironmentNotSet_NoServiceAdded()
        {
            Environment.SetEnvironmentVariable("ISTIO_ENVOY_API", null);

            var host = _builder.UseIstioSidecar().Build();

            Assert.That(host.Services.GetServices<IHostedService>(), Has.Exactly(0).Matches(Is.TypeOf<IstioLifetimeService>()));
        }

        [Test]
        public void UseIstioSidecar_EnvironmentSet_LifetimeServiceAdded()
        {
            Environment.SetEnvironmentVariable("ISTIO_ENVOY_API", "http://localhost:15000");

            var host = _builder.UseIstioSidecar().Build();

            Assert.That(host.Services.GetServices<IHostedService>(), Has.Exactly(1).Matches(Is.TypeOf<IstioLifetimeService>()));
        }
    }
}
