using Microsoft.Extensions.Logging;
using Portic.Consumers;
using Portic.Transport.InMemory.Messages;
using Portic.Transport.InMemory.Transport;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.InMemory.Consumers
{
    internal sealed class InMemoryConsumerExecutor(
        InMemoryTransport _transport,
        IConsumerExecutor _consumerExecutor,
        ILoggerFactory _loggerFactory
    )
    {
        private delegate Task ConsumeDelegate(InMemoryConsumerExecutor instance, InMemoryQueuedMessage message, CancellationToken cancellationToken);

        private readonly ILogger<InMemoryMessageSettlement> MessageSettlementLogger = _loggerFactory.CreateLogger<InMemoryMessageSettlement>();

        private static readonly MethodInfo ConsumeMethodInfo;

        private static readonly ConcurrentDictionary<Type, ConsumeDelegate> MessageTypeDelegates = [];

        static InMemoryConsumerExecutor()
        {
            ConsumeMethodInfo = typeof(InMemoryConsumerExecutor).GetMethod(nameof(ConsumeAsync), BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new Exception("Failed to find ConsumeAsync method.");
        }

        public async ValueTask ExecuteAsync(InMemoryQueuedMessage message, CancellationToken cancellationToken = default)
        {
            var consumeDelegate = GetOrCreateConsumeDelegate(message.MessageDefinition.MessageType);

            await consumeDelegate(this, message, cancellationToken);
        }

        private async Task ConsumeAsync<TMessage>(InMemoryQueuedMessage message, CancellationToken cancellationToken)
        {
            var settlement = new InMemoryMessageSettlement(
                message,
                _transport,
                MessageSettlementLogger
            );

            var messageReceived = new TransportMessageReceived<TMessage>(
                message.MessageId,
                (TMessage)message.Message,
                message.DeliveryCount,
                message.ConsumerDefinition,
                message.EndpointDefinition,
                settlement
            );

            await _consumerExecutor.ExecuteAsync(messageReceived, cancellationToken);
        }

        private static ConsumeDelegate GetOrCreateConsumeDelegate(Type messageType)
        {
            return MessageTypeDelegates.GetOrAdd(
                messageType,
                static type => ConsumeMethodInfo.MakeGenericMethod(type).CreateDelegate<ConsumeDelegate>()
            );
        }
    }
}