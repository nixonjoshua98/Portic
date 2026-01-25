using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Consumer;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyFactory(
        IRabbitMQConnectionContext _connectionContext,
        IRabbitMQMessageConsumer _messageConsumer
    ) : IRabbitMQTopologyFactory
    {
        public async Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointConfiguration endpoint, CancellationToken cancellationToken)
        {
            var channel = await _connectionContext.CreateChannelAsync(
                endpoint.CreateChannelOptions(),
                cancellationToken
            );

            var queue = await channel.QueueDeclareAsync(endpoint, cancellationToken);

            var state = new RabbitMQEndpointState(
                channel, 
                endpoint,
                _messageConsumer.ConsumeAsync,
                cancellationToken
            );

            foreach (var (_, consumer) in endpoint.Consumers)
            {
                await channel.ExchangeDeclareAsync(consumer.Message.Name, ExchangeType.Fanout, cancellationToken: cancellationToken);

                await channel.QueueBindAsync(queue.QueueName, consumer.Message.Name, string.Empty, cancellationToken: cancellationToken);
            }

            await state.BasicConsumeAsync();

            return state;
        }
    }
}