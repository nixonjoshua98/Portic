using Microsoft.Extensions.Logging;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Channels
{
    internal sealed class RabbitMQChannel(IChannel channel, ILogger logger, RabbitMQChannelPool? channelPool = null) : IDisposable
    {
        private bool _isDisposed;

        private readonly IChannel _channel = channel;

        private readonly RabbitMQChannelPool? ChannelPool = channelPool;
        private readonly ILogger Logger = logger;

        public IChannel RawChannel
        {
            get
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                return _channel;
            }
        }

        public async Task BindConsumerToEndpointAsync(IEndpointDefinition endpoint, IConsumerDefinition consumer, CancellationToken cancellationToken)
        {
            await RawChannel.ExchangeDeclareAsync(
                exchange: consumer.Message.Name,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await _channel.QueueDeclareAsync(
                queue: endpoint.Name,
                durable: endpoint.Durable,
                exclusive: endpoint.Exclusive,
                autoDelete: endpoint.AutoDelete,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: consumer.Message.Name,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );

            Logger.LogQueueBinding(consumer.Message.Name, queue.QueueName);
        }

        public async Task BindFaultedQueueAsync(IMessageDefinition messageDefinition, IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            await _channel.ExchangeDeclareAsync(
                exchange: messageDefinition.FaultedExchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await _channel.QueueDeclareAsync(
                queue: endpointDefinition.FaultedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: messageDefinition.FaultedExchangeName,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );
        }

        public ValueTask BasicAckAsync(ulong deliveryTag, bool multiple, CancellationToken cancellationToken = default)
        {
            return _channel.BasicAckAsync(deliveryTag, multiple, cancellationToken);
        }

        public Task<string> BasicConsumeAsync(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object?>? arguments, IAsyncBasicConsumer consumer, CancellationToken cancellationToken = default)
        {
            return _channel.BasicConsumeAsync(queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer, cancellationToken);
        }

        public ValueTask BasicPublishAsync<TProperties>(string exchange, string routingKey, bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
        {
            return _channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, cancellationToken);
        }

        public Task BasicQosAsync(uint prefetchSize, ushort prefetchCount, bool global, CancellationToken cancellationToken = default)
        {
            return _channel.BasicQosAsync(prefetchSize, prefetchCount, global, cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (ChannelPool is not null)
                    {
                        ChannelPool.Release(_channel);
                    }
                    else
                    {
                        _channel?.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
