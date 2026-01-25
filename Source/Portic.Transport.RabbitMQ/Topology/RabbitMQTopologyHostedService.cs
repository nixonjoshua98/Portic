using Microsoft.Extensions.Hosting;
using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyHostedService(
        IPorticConfiguration _configuration,
        IRabbitMQTopologyFactory _topologyFactory
    ) : IHostedLifecycleService
    {
        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            foreach (var endpoint in _configuration.Endpoints)
            {
                var state = await _topologyFactory.CreateEndpointStateAsync(endpoint, cancellationToken);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}