using Microsoft.Extensions.Hosting;
using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Consumer;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyHostedService(
        IPorticConfiguration _configuration,
        IRabbitMQTopologyFactory _topologyFactory
    ) : IHostedLifecycleService
    {
        private readonly List<RabbitMQEndpointState> _endpointStates = [];

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            foreach (var endpoint in _configuration.Endpoints)
            {
                var state = await _topologyFactory.CreateEndpointStateAsync(endpoint, cancellationToken);

                _endpointStates.Add(state);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            DisposeEndpointStates();
        }

        private void DisposeEndpointStates()
        {
            foreach (var state in _endpointStates)
            {
                try
                {
                    state.Dispose();
                }
                catch (Exception)
                {
                    // Swallow
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}