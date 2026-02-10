using Portic.Consumers;
using Portic.Serializer;
using Portic.Transport.RabbitMQ.Messages;
using Portic.Transport.RabbitMQ.Topology;
using System.Collections.Concurrent;
using System.Reflection;

namespace Portic.Transport.RabbitMQ.Consumers
{
    internal sealed class RabbitMQConsumerExecutor(
        IPorticSerializer _serializer,
        IConsumerExecutor _consumerExecutor,
        IRabbitMQTransport _transport
    )
    {
        private delegate Task ConsumeDelegate(RabbitMQConsumerExecutor instance, RabbitMQRawMessageReceived message, CancellationToken cancellationToken);

        private static readonly MethodInfo ConsumeMethodInfo;

        private static readonly ConcurrentDictionary<Type, ConsumeDelegate> MessageTypeDelegates = [];

        static RabbitMQConsumerExecutor()
        {
            ConsumeMethodInfo = typeof(RabbitMQConsumerExecutor).GetMethod(nameof(ConsumeAsync), BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new Exception("Failed to find ConsumeAsync method.");
        }

        public async Task ExecuteAsync(RabbitMQRawMessageReceived message, CancellationToken cancellationToken)
        {
            var consumeDelegate = GetOrCreateConsumeDelegate(message.MessageDefinition.MessageType);

            await consumeDelegate(this, message, cancellationToken);
        }

        private async Task ConsumeAsync<TMessage>(RabbitMQRawMessageReceived message, CancellationToken cancellationToken)
        {
            var body = _serializer.Deserialize<RabbitMQMessageBody<TMessage>>(message.RawBody.Span);

            var settlement = new RabbitMQMessageSettlement<TMessage>(message, _transport);

            var messageReceived = new TransportMessageReceived<TMessage>(
                message.MessageId!,
                body.Message,
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