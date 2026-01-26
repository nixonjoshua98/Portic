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
            var configuration = _configuration.GetMessageConfiguration<TMessage>();

            var body = new RabbitMQMessageBody<TMessage>(
                Guid.CreateVersion7().ToString(),
                message
            );

            var payloadBytes = _serializer.SerializeToBytes(body);

            var properties = new BasicProperties()
                .SetMessageName(configuration.Name);

            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.BasicPublishAsync(
                configuration.Name,
                string.Empty,
                mandatory: configuration.Mandatory,
                properties,
                payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}