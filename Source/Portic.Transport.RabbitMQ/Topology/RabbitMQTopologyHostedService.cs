using Microsoft.Extensions.Hosting;
using Portic.Configuration;
using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Extensions;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyHostedService(
        IPorticConfiguration _configuration,
        RabbitMQConnectionContext _connectionContext,
        RabbitMQConsumerExecutor _consumerExecutor,
        RabbitMQTopologyService _topologyService
    ) : IHostedLifecycleService
    {
        private readonly List<RabbitMQEndpointState> _endpointStates = [];

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            foreach (var endpoint in _configuration.Endpoints)
            {
                var state = await CreateEndpointStateAsync(endpoint, cancellationToken);

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

        private async Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointDefinition endpoint, CancellationToken cancellationToken)
        {
            foreach (var consumer in endpoint.ConsumerDefinitions)
            {
                await _topologyService.BindQueueAsync(endpoint, consumer, cancellationToken);
            }

            var state = new RabbitMQEndpointState(
                endpoint,
                _consumerExecutor.ExecuteAsync
            );

            for (int i = 0; i < endpoint.ChannelCount; i++)
            {
                var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

                await channel.BasicQosAsync(0, endpoint.PrefetchCount, global: false, cancellationToken);

                var consumerState = state.AddConsumer(channel);
            }

            return state;
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