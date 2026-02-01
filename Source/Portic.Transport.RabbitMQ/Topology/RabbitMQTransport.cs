using Portic.Configuration;
using Portic.Messages;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Messages;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTransport(
        RabbitMQConnectionContext _connectionContext,
        IPorticConfiguration _configuration,
        IPorticSerializer _serializer
    ) : IRabbitMQTransport
    {
        public async Task PublishFaultedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken)
        {
            var properties = new BasicProperties()
                .CopyHeadersFrom(message.BasicProperties)
                .SetException(exception)
                .SetDeliveryCount(Convert.ToByte(message.DeliveryCount + 1));

            await PublishAsync(
                message.MessageConfiguration,
                message.RawBody,
                properties,
                cancellationToken
            );
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            var definition = _configuration.GetMessageDefinition<TMessage>();

            var body = new RabbitMQMessageBody<TMessage>(
                message
            );

            var payloadBytes = _serializer.Serialize(body);

            var properties = new BasicProperties()
                .SetMessageId(Guid.CreateVersion7().ToString())
                .SetMessageName(definition.Name);

            await PublishAsync(
                definition,
                payloadBytes,
                properties,
                cancellationToken
            );
        }

        private async Task PublishAsync(
            IMessageDefinition configuration,
            ReadOnlyMemory<byte> payloadBytes,
            BasicProperties properties,
            CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.BasicPublishAsync(
                exchange: configuration.Name,
                routingKey: string.Empty,
                mandatory: configuration.Mandatory,
                basicProperties: properties,
                body: payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}