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
            var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

            var queue = await channel.QueueDeclareAsync(endpoint, cancellationToken);

            var state = new RabbitMQEndpointState(
                channel, 
                endpoint,
                _messageConsumer.ConsumeAsync,
                cancellationToken
            );

            foreach (var (_, consumer) in endpoint.Consumers)
            {
                var exchangeName = consumer.Message.GetName();

                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, cancellationToken: cancellationToken);

                await channel.QueueBindAsync(queue.QueueName, exchangeName, string.Empty, cancellationToken: cancellationToken);
            }

            await state.BasicConsumeAsync();

            return state;
        }
    }
}