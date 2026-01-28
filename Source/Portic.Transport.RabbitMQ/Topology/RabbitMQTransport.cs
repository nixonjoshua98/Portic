using Portic.Abstractions;
using Portic.Consumer;
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
    ) : IRabbitMQTransport
    {
        public async Task RePublishAsync<TMessage>(IConsumerContext<TMessage> context, CancellationToken cancellationToken)
        {
            var body = new RabbitMQMessageBody<TMessage>(
                context.MessageId,
                context.Message
            );

            var payloadBytes = _serializer.SerializeToBytes(body);

            var properties = new BasicProperties()
                .SetMessageName(context.MessageName)
                .SetDeliveryCount(Convert.ToByte(context.DeliveryCount + 1));

            await PublishAsync(
                context.MessageConfiguration,
                payloadBytes,
                properties,
                cancellationToken
            );
        }

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

            await PublishAsync(
                configuration, 
                payloadBytes, 
                properties, 
                cancellationToken
            );
        }

        private async Task PublishAsync(
            IMessageConfiguration configuration, 
            byte[] payloadBytes,
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