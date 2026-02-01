using Microsoft.Extensions.Hosting;
using Portic.Configuration;
using Portic.Transport.RabbitMQ.Consumers;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyHostedService(
        IPorticConfiguration _configuration,
        RabbitMQEndpointFactory _endpointFactory
    ) : IHostedLifecycleService
    {
        private readonly List<RabbitMQEndpointState> _endpointStates = [];

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            foreach (var endpoint in _configuration.Endpoints)
            {
                var state = await _endpointFactory.CreateEndpointStateAsync(endpoint, cancellationToken);

                _endpointStates.Add(state);
            }

            // Start consumers after all endpoints have been created

            foreach (var state in _endpointStates)
            {
                foreach (var consumer in state.Consumers)
                {
                    await consumer.BasicConsumeAsync(cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            DisposeEndpointStates();

            return Task.CompletedTask;
        }

        private void DisposeEndpointStates()
        {
            _endpointStates.ForEach(x => x?.Dispose());
            _endpointStates.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}