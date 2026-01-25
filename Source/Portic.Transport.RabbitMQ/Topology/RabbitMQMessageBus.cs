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

            var properties = new BasicProperties()
                .SetMessageName(messageConfiguration.Name);

            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.BasicPublishAsync(
                messageConfiguration.Name,
                string.Empty,
                mandatory: true,
                properties,
                payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}
