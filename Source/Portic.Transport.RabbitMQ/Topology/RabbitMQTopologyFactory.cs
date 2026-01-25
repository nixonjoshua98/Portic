using Portic.Consumer;
using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyFactory(
        IRabbitMQConnectionContext _connectionContext
    ) : IRabbitMQTopologyFactory
    {
        public async Task CreateTopologyAsync(IEnumerable<IMessageConsumerConfiguration> consumers, CancellationToken cancellationToken)
        {
            await using var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

            foreach (var consumer in consumers)
            {
                await DeclareAndBindConsumerAsync(channel, consumer, cancellationToken);
            }
        }

        private static async Task DeclareAndBindConsumerAsync(IChannel channel, IMessageConsumerConfiguration consumer, CancellationToken cancellationToken)
        {
            var exchangeName = consumer.Message.GetName();

            await channel.ExchangeDeclareAsync(
                exchangeName,
                ExchangeType.Fanout,
                cancellationToken: cancellationToken
            );

            var queue = await channel.QueueDeclareAsync(consumer, cancellationToken);

            await channel.QueueBindAsync(
                queue.QueueName,
                exchangeName,
                string.Empty,
                cancellationToken: cancellationToken
            );
        }
    }
}