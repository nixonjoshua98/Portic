using Microsoft.Extensions.Logging;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Transport
{
    internal sealed class RabbitMQTopologyService(
        RabbitMQConnectionContext _connectionContext,
        ILogger<RabbitMQTopologyService> _logger
    )
    {
        public async Task BindQueueAsync(IEndpointDefinition endpoint, IConsumerDefinition consumer, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(
                exchange: consumer.Message.Name,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await rented.Channel.QueueDeclareAsync(
                queue: endpoint.Name,
                durable: endpoint.Durable,
                exclusive: endpoint.Exclusive,
                autoDelete: endpoint.AutoDelete,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: consumer.Message.Name,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );

            _logger.LogQueueBinding(consumer.Message.Name, queue.QueueName);
        }

        public async Task BindFaultedQueueAsync(IMessageDefinition messageDefinition, IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            using var rented = await _connectionContext.RentChannelAsync(cancellationToken);

            await rented.Channel.ExchangeDeclareAsync(
                exchange: messageDefinition.FaultedExchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await rented.Channel.QueueDeclareAsync(
                queue: endpointDefinition.FaultedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await rented.Channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: messageDefinition.FaultedExchangeName,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );
        }
    }
}
