using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portic.Consumer;
using Portic.Exceptions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Logging;
using Portic.Transport.RabbitMQ.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQConsumerExecutor(
        IServiceScopeFactory _scopeFactory,
        IPorticSerializer _serializer,
        ILogger<RabbitMQConsumerExecutor> _logger,
        IConsumerExecutor _consumerExecutor
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
            try
            {
                if (!message.TryGetConsumerConfiguration(out var consumerConfig))
                {
                    throw UnknownMessageException.FromName(message.MessageName);
                }

                await ExecuteConsumerAsync(message, consumerConfig, cancellationToken);

                await message.Channel.BasicAckAsync(message.DeliveryTag, false, cancellationToken);

                _logger.LogMessageConsumed(
                    consumerConfig.Message.Name,
                    message.EndpointConfiguration.Name
                );
            }
            catch (Exception)
            {
                await message.Channel.BasicNackAsync(message.DeliveryTag, false, true, cancellationToken);

                throw;
            }
        }

        private async Task ExecuteConsumerAsync(TransportMessageReceived message, IConsumerConfiguration consumerConfig, CancellationToken cancellationToken)
        {
            var genericConsumeMethod = GetGenericConsumeMethod(consumerConfig.Message.MessageType);

            object[] methodArgs = [message, consumerConfig, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task;

            await genericConsumeResult!;
        }

        private async Task ConsumeGenericAsync<TMessage>(
            TransportMessageReceived message,
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
                cancellationToken
            );

            await _consumerExecutor.ExecuteAsync(context, cancellationToken);
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
