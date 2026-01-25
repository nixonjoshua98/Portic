using Microsoft.Extensions.DependencyInjection;
using Portic.Abstractions;
using Portic.Consumer;
using Portic.Exceptions;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQMessageConsumer(
        IServiceScopeFactory _scopeFactory,
        IPorticConfiguration _configuration,
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

        public async Task ConsumeAsync(BasicDeliverEventArgs args, CancellationToken cancellationToken)
        {
            var messageName = args.GetMessageName();

            if (string.IsNullOrEmpty(messageName))
            {
                throw InvalidMessageNameException.FromName(messageName);
            }

            var messageConfiguration = _configuration.GetMessageConfiguration(messageName);

            var consumerConfiguration = _configuration.GetConsumerForMessage(messageConfiguration)
                ?? throw MessageConsumerNotFoundException.FromName(messageName);

            await ExecuteConsumeGenericAsync(args, consumerConfiguration, cancellationToken);
        }

        private async Task ExecuteConsumeGenericAsync(
            BasicDeliverEventArgs args,
            IMessageConsumerConfiguration consumerConfiguration,
            CancellationToken cancellationToken)
        {
            var messageConfiguration = consumerConfiguration.Message;

            var messageName = messageConfiguration.GetName();

            var genericConsumeMethod = GetGenericConsumeMethod(messageConfiguration.MessageType);

            object[] methodArgs = [messageName, args, consumerConfiguration, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task
                ?? throw MessageConsumerNotFoundException.FromName(messageName);

            await genericConsumeResult;
        }

        private async Task ConsumeGenericAsync<TMessage>(
            string messageName,
            BasicDeliverEventArgs args,
            IMessageConsumerConfiguration consumerConfiguration,
            CancellationToken cancellationToken)
        {
            var payload = _serializer.Deserialize<TransportPayload<TMessage>>(args.Body.Span);

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
