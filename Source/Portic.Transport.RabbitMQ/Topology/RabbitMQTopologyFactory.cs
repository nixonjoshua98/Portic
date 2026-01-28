using Microsoft.Extensions.Logging;
using Portic.Consumer;
using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Logging;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyFactory(
        IRabbitMQConnectionContext _connectionContext,
        IRabbitMQConsumerExecutor _messageConsumer,
        ILogger<RabbitMQTopologyFactory> _logger
    ) : IRabbitMQTopologyFactory
    {
        public async Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointConfiguration endpoint, CancellationToken cancellationToken)
        {
            foreach (var (_, consumer) in endpoint.Consumers)
            {
                await BindQueuesToExchangeAsync(endpoint, consumer, cancellationToken);
            }

            var state = new RabbitMQEndpointState(
                endpoint,
                _messageConsumer.ExecuteAsync
            );

            for (int i = 0; i < endpoint.ChannelCount; i++)
            {
                var channel = await _connectionContext.CreateChannelAsync(
                    endpoint.CreateChannelOptions(),
                    cancellationToken
                );

                var consumerState = state.AddConsumer(channel);
            }

            return state;
        }

        private async Task BindQueuesToExchangeAsync(IEndpointConfiguration endpoint, IConsumerConfiguration consumer, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            var queue = await rented.Channel.QueueDeclareAsync(endpoint, cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(consumer.Message.Name, ExchangeType.Fanout, cancellationToken: cancellationToken);

            await rented.Channel.QueueBindAsync(queue.QueueName, consumer.Message.Name, string.Empty, cancellationToken: cancellationToken);

            _logger.LogExchangeBoundToQueue(queue.QueueName, consumer.Message.Name);
        }
    }
}