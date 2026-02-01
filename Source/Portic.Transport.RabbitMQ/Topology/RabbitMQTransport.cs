using Microsoft.Extensions.Logging;
using Portic.Configuration;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Logging;
using Portic.Transport.RabbitMQ.Messages;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTransport(
        IRabbitMQConnectionContext _connectionContext,
        IPorticConfiguration _configuration,
        IPorticSerializer _serializer,
        IRabbitMQConsumerExecutor _messageConsumer,
        ILogger<RabbitMQTransport> _logger
    ) : IRabbitMQTransport
    {
        public async Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointDefinition endpoint, CancellationToken cancellationToken)
        {
            foreach (var (_, consumer) in endpoint.Consumers)
            {
                await BindQueuesToExchangeAsync(endpoint, consumer, cancellationToken);
            }

            var state = new RabbitMQEndpointState(
                endpoint,
                _messageConsumer.ExecuteAsync
            );

            for (int i = 0; i < endpoint.ChannelCount; i++)
            {
                var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

                await channel.BasicQosAsync(0, endpoint.PrefetchCount, global: false, cancellationToken);

                var consumerState = state.AddConsumer(channel);
            }

            return state;
        }

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

        private async Task BindQueuesToExchangeAsync(IEndpointDefinition endpoint, IConsumerDefinition consumer, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            var queue = await rented.Channel.QueueDeclareAsync(endpoint, cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(
                exchange: consumer.Message.Name,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueBindAsync(queue.QueueName, consumer.Message.Name, string.Empty, cancellationToken: cancellationToken);

            _logger.LogExchangeBoundToQueue(queue.QueueName, consumer.Message.Name);
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