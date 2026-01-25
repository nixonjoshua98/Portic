using Portic.Abstractions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQMessageBus(
        IRabbitMQConnectionContext _connectionContext,
        IPorticConfiguration _configuration,
        IPorticSerializer _serializer
    ) : IMessageBus
    {
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            var messageConfiguration = _configuration.GetMessageConfiguration<TMessage>();

            var payload = new TransportMessagePayload<TMessage>(
                message,
                DateTimeOffset.UtcNow
            );

            var payloadBytes = _serializer.SerializeToBytes(payload);

            var messageName = messageConfiguration.GetName();

            var properties = new BasicProperties()
                .SetMessageName(messageName);

            await using var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

            await channel.BasicPublishAsync(
                messageName,
                string.Empty,
                mandatory: true,
                properties,
                payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}
