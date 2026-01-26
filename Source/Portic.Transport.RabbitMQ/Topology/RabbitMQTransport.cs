using Portic.Abstractions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTransport(
        IRabbitMQConnectionContext _connectionContext,
        IPorticConfiguration _configuration,
        IPorticSerializer _serializer
    ) : IMessageTransport
    {
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            var messageConfiguration = _configuration.GetMessageConfiguration<TMessage>();

            var payload = new TransportMessagePayload<TMessage>(
                Guid.CreateVersion7().ToString(),
                message
            );

            var payloadBytes = _serializer.SerializeToBytes(payload);

            var properties = new BasicProperties()
                .SetMessageName(messageConfiguration.Name);

            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.BasicPublishAsync(
                messageConfiguration.Name,
                string.Empty,
                mandatory: false,
                properties,
                payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}