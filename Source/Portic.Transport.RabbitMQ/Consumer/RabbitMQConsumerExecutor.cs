using Microsoft.Extensions.Logging;
using Portic.Consumer;
using Portic.Exceptions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Logging;
using Portic.Transport.RabbitMQ.Models;
using Portic.Transport.RabbitMQ.Topology;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQConsumerExecutor(
        IPorticSerializer _serializer,
        ILogger<RabbitMQConsumerExecutor> _logger,
        IConsumerExecutor _consumerExecutor,
        IRabbitMQTransport _transport
    ) : IRabbitMQConsumerExecutor
    {
        private static readonly MethodInfo ConsumeGenericMethodInfo;

        private static readonly ConcurrentDictionary<Type, MethodInfo> MessageTypeConsumeMethods = [];

        static RabbitMQConsumerExecutor()
        {
            ConsumeGenericMethodInfo = typeof(RabbitMQConsumerExecutor).GetMethod(nameof(ConsumeGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new Exception("Failed to find ConsumeGenericAsync method.");
        }

        public async Task ExecuteAsync(RawTransportMessageReceived message, CancellationToken cancellationToken)
        {
            var genericConsumeMethod = GetGenericConsumeMethod(message.MessageConfiguration.MessageType);

            object[] methodArgs = [message, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task;

            await genericConsumeResult!;
        }

        private async Task ConsumeGenericAsync<TMessage>(RawTransportMessageReceived message, CancellationToken cancellationToken)
        {
            var body = _serializer.Deserialize<RabbitMQMessageBody<TMessage>>(message.RawBody.Span);

            var messageReceived = message.ToReceivedMessage(body.MessageId, body.Message);

            try
            {
                await _consumerExecutor.ExecuteAsync(messageReceived, cancellationToken);

                await message.Channel.BasicAckAsync(message.DeliveryTag, false, cancellationToken);
            }

            catch (PorticConsumerException<TMessage> ex) when (ex.ShouldRedeliver)
            {
                await RedeliverMessageAsync(message, ex.Context, cancellationToken);
            }

            catch (Exception)
            {
                // DLQ

                await message.Channel.BasicNackAsync(message.DeliveryTag, false, false, cancellationToken);

                throw;
            }
        }

        private async Task RedeliverMessageAsync<TMessage>(
            RawTransportMessageReceived message,
            IConsumerContext<TMessage> context,
            CancellationToken cancellationToken)
        {
            // Republish the message for redelivery first, to ensure at-least-once delivery guarantee
            await _transport.RePublishAsync(context, cancellationToken);

            // Nack the original message without requeueing, since we've already republished it
            // This prevents potential duplicate deliveries from the original queue
            // Intentionally ignoring cancellationToken here to ensure Nack is sent regardless of cancellation
            await message.Channel.BasicNackAsync(message.DeliveryTag, false, false, CancellationToken.None);

            _logger.LogSuccessfulRedelivery(context.MessageId, context.DeliveryCount + 1, context.MaxRedeliveryAttempts);
        }

        private static MethodInfo GetGenericConsumeMethod(Type messageType)
        {
            return MessageTypeConsumeMethods.GetOrAdd(
                messageType,
                _ => ConsumeGenericMethodInfo.MakeGenericMethod(messageType)
            );
        }
    }
}
