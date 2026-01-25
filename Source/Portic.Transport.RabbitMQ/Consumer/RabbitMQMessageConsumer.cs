using Microsoft.Extensions.DependencyInjection;
using Portic.Consumer;
using Portic.Exceptions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQMessageConsumer(
        IServiceScopeFactory _scopeFactory,
        IMessageConsumerContextFactory _contextFactory,
        IPorticSerializer _serializer
    ) : IRabbitMQMessageConsumer
    {
        private static readonly MethodInfo ConsumeGenericMethodInfo;

        private static readonly ConcurrentDictionary<Type, MethodInfo> MessageTypeConsumeMethods = [];

        static RabbitMQMessageConsumer()
        {
            ConsumeGenericMethodInfo = typeof(RabbitMQMessageConsumer).GetMethod(nameof(ConsumeGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new Exception("Failed to find ConsumeGenericAsync method.");
        }

        public async Task ConsumeAsync(TransportMessageReceived message, CancellationToken cancellationToken)
        {
            if (!message.TryGetConsumerConfiguration(out var consumerConfiguration))
            {
                throw MessageConsumerNotFoundException.FromName(message.MessageName);
            }

            var messageConfig = consumerConfiguration.Message;

            var genericConsumeMethod = GetGenericConsumeMethod(messageConfig.MessageType);

            object[] methodArgs = [messageConfig.Name, message, consumerConfiguration, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task;

            await genericConsumeResult!;
        }

        private async Task ConsumeGenericAsync<TMessage>(
            string messageName,
            TransportMessageReceived message,
            IConsumerConfiguration consumerConfiguration,
            CancellationToken cancellationToken)
        {
            var payload = _serializer.Deserialize<TransportMessagePayload<TMessage>>(message.Body);

            await using var scope = _scopeFactory.CreateAsyncScope();

            var context = _contextFactory.CreateContext(
                payload,
                cancellationToken
            );

            var consumerInst = ActivatorUtilities.CreateInstance(scope.ServiceProvider, consumerConfiguration.ConsumerType) as IMessageConsumer<TMessage>
                ?? throw MessageConsumerNotFoundException.FromName(messageName);

            await consumerInst.ConsumeAsync(context);
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
