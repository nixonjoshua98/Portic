using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portic.Consumer;
using Portic.Endpoint;
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
        IServiceScopeFactory _scopeFactory,
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

        public async Task ExecuteAsync(TransportMessageReceived message, CancellationToken cancellationToken)
        {
            if (!message.TryGetConsumerConfiguration(out var consumerConfig))
            {
                throw MessageTypeNotFoundException.FromName(message.MessageName);
            }

            var genericConsumeMethod = GetGenericConsumeMethod(consumerConfig.Message.MessageType);

            object[] methodArgs = [message, message.EndpointConfiguration, consumerConfig, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task;

            await genericConsumeResult!;
        }

        private async Task ConsumeGenericAsync<TMessage>(
            TransportMessageReceived message,
            IEndpointConfiguration endpointConfiguration,
            IConsumerConfiguration consumerConfiguration,
            CancellationToken cancellationToken
        )
        {
            var body = _serializer.Deserialize<RabbitMQMessageBody<TMessage>>(message.Body);

            await using var scope = _scopeFactory.CreateAsyncScope();

            var context = new ConsumerContext<TMessage>(
                body.MessageId,
                body.Message,
                message.DeliveryCount,
                scope.ServiceProvider,
                consumerConfiguration,
                endpointConfiguration,
                cancellationToken
            );

            try
            {
                await _consumerExecutor.ExecuteAsync(context, cancellationToken);

                await message.Channel.BasicAckAsync(message.DeliveryTag, false, cancellationToken);

                _logger.LogMessageConsumed(context.MessageName, message.EndpointConfiguration.Name);
            }

            catch (PorticConsumerException ex) when (ex.ShouldRedeliver)
            {
                await RedeliverMessageAsync(message, context, cancellationToken);
            }

            catch (Exception)
            {
                // DLQ

                await message.Channel.BasicNackAsync(message.DeliveryTag, false, false, cancellationToken);

                throw;
            }
        }

        private async Task RedeliverMessageAsync<T>(TransportMessageReceived message, ConsumerContext<T> context, CancellationToken cancellationToken)
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
