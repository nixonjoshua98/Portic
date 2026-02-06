using Portic.Configuration;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Messages;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTransport(
        RabbitMQConnectionContext _connectionContext,
        RabbitMQTopologyService _topologyService,
        IPorticConfiguration _configuration,
        IPorticSerializer _serializer
    ) : IRabbitMQTransport
    {
        public async Task PublishDeferedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken)
        {
            var properties = new BasicProperties()
                .CopyHeadersFrom(message.BasicProperties)
                .SetException(exception)
                .SetDeliveryCount(Convert.ToByte(message.DeliveryCount + 1));

            await PublishAsync(
                message.MessageDefinition.Name,
                message.MessageDefinition.Mandatory,
                message.RawBody,
                properties,
                cancellationToken
            );
        }

        public async Task PublishFaultedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken)
        {
            var properties = new BasicProperties()
                .CopyHeadersFrom(message.BasicProperties)
                .SetException(exception)
                .SetDeliveryCount(Convert.ToByte(message.DeliveryCount + 1));

            var exchaneName = message.MessageDefinition.Name + "-faulted";
            var queueName = message.EndpointDefinition.Name + "-faulted";

            await _topologyService.BindFaultedQueueAsync(exchaneName, queueName, cancellationToken);

            await PublishAsync(
                exchaneName,
                message.MessageDefinition.Mandatory,
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
                definition.Name,
                definition.Mandatory,
                payloadBytes,
                properties,
                cancellationToken
            );
        }

        private async Task PublishAsync(
            string exchange,
            bool mandatory,
            ReadOnlyMemory<byte> payloadBytes,
            BasicProperties properties,
            CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: string.Empty,
                mandatory: mandatory,
                basicProperties: properties,
                body: payloadBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}