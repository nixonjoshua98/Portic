using Microsoft.Extensions.Logging;
using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport.RabbitMQ.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.Channels
{
    internal sealed class RabbitMQChannel(IChannel channel, ILogger logger, RabbitMQChannelPool? channelPool = null) : IDisposable
    {       
        private bool _isDisposed;

        private readonly RabbitMQChannelPool? ChannelPool = channelPool;
        private readonly IChannel Channel = channel;
        private readonly ILogger Logger = logger;

        public IChannel RawChannel
        {
            get
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                return Channel;
            }
        }

        public async Task BindConsumerToEndpointAsync(IEndpointDefinition endpoint, IConsumerDefinition consumer, CancellationToken cancellationToken)
        {
            await Channel.ExchangeDeclareAsync(
                exchange: consumer.Message.Name,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await Channel.QueueDeclareAsync(
                queue: endpoint.Name,
                durable: endpoint.Durable,
                exclusive: endpoint.Exclusive,
                autoDelete: endpoint.AutoDelete,
                cancellationToken: cancellationToken
            );

            await Channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: consumer.Message.Name,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );

            Logger.LogQueueBinding(consumer.Message.Name, queue.QueueName);
        }

        public async Task BindFaultedQueueAsync(IMessageDefinition messageDefinition, IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            await Channel.ExchangeDeclareAsync(
                exchange: messageDefinition.FaultedExchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            var queue = await Channel.QueueDeclareAsync(
                queue: endpointDefinition.FaultedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await Channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: messageDefinition.FaultedExchangeName,
                routingKey: string.Empty,
                cancellationToken: cancellationToken
            );
        }

        public Task ExchangeBindAsync(string destination, string source, string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false, CancellationToken cancellationToken = default)
        {
            return Channel.ExchangeBindAsync(destination, source, routingKey, arguments, noWait, cancellationToken);
        }

        public Task ExchangeDeclareAsync(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false, CancellationToken cancellationToken = default)
        {
            return Channel.ExchangeDeclareAsync(exchange, type, durable, autoDelete, arguments, passive, noWait, cancellationToken);
        }

        public ValueTask BasicAckAsync(ulong deliveryTag, bool multiple, CancellationToken cancellationToken = default)
        {
            return Channel.BasicAckAsync(deliveryTag, multiple, cancellationToken);
        }

        public ValueTask BasicNackAsync(ulong deliveryTag, bool multiple, bool requeue, CancellationToken cancellationToken = default)
        {
            return Channel.BasicNackAsync(deliveryTag, multiple, requeue, cancellationToken);
        }

        public Task<string> BasicConsumeAsync(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object?>? arguments, IAsyncBasicConsumer consumer, CancellationToken cancellationToken = default)
        {
            return Channel.BasicConsumeAsync(queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer, cancellationToken);
        }

        public ValueTask BasicPublishAsync<TProperties>(string exchange, string routingKey, bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
        {
            return Channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, cancellationToken);
        }

        public ValueTask BasicPublishAsync<TProperties>(CachedString exchange, CachedString routingKey, bool mandatory, TProperties basicProperties, ReadOnlyMemory<byte> body, CancellationToken cancellationToken = default) where TProperties : IReadOnlyBasicProperties, IAmqpHeader
        {
            return Channel.BasicPublishAsync(exchange, routingKey, mandatory, basicProperties, body, cancellationToken);
        }

        public Task BasicQosAsync(uint prefetchSize, ushort prefetchCount, bool global, CancellationToken cancellationToken = default)
        {
            return Channel.BasicQosAsync(prefetchSize, prefetchCount, global, cancellationToken);
        }

        public ValueTask BasicRejectAsync(ulong deliveryTag, bool requeue, CancellationToken cancellationToken = default)
        {
            return Channel.BasicRejectAsync(deliveryTag, requeue, cancellationToken);
        }

        public Task<QueueDeclareOk> QueueDeclareAsync(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false, CancellationToken cancellationToken = default)
        {
            return Channel.QueueDeclareAsync(queue, durable, exclusive, autoDelete, arguments, passive, noWait, cancellationToken);
        }

        public Task QueueBindAsync(string queue, string exchange, string routingKey, IDictionary<string, object?>? arguments = null, bool noWait = false, CancellationToken cancellationToken = default)
        {
            return Channel.QueueBindAsync(queue, exchange, routingKey, arguments, noWait, cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (ChannelPool is not null)
                    {
                        ChannelPool.Release(Channel);
                    }
                    else
                    {
                        Channel?.Dispose();
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
