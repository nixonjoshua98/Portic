using Microsoft.Extensions.Logging;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Consumers;
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
        public async Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointDefinition endpoint, CancellationToken cancellationToken)
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

        private async Task BindQueuesToExchangeAsync(IEndpointDefinition endpoint, IConsumerDefinition consumer, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            var queue = await rented.Channel.QueueDeclareAsync(endpoint, cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(
                exchange: consumer.Message.Name, 
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueBindAsync(queue.QueueName, consumer.Message.Name, string.Empty, cancellationToken: cancellationToken);

            _logger.LogExchangeBoundToQueue(queue.QueueName, consumer.Message.Name);
        }
    }
}