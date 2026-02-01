using Microsoft.Extensions.Logging;
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
        ILogger<RabbitMQConsumerExecutor> _logger,
        IConsumerExecutor _consumerExecutor,
        IRabbitMQTransport _transport
    )
    {
        private static readonly MethodInfo ConsumeGenericMethodInfo;

        private static readonly ConcurrentDictionary<Type, MethodInfo> MessageTypeConsumeMethods = [];

        static RabbitMQConsumerExecutor()
        {
            ConsumeGenericMethodInfo = typeof(RabbitMQConsumerExecutor).GetMethod(nameof(ConsumeGenericAsync), BindingFlags.NonPublic | BindingFlags.Instance) ??
                throw new Exception("Failed to find ConsumeGenericAsync method.");
        }

        public async Task ExecuteAsync(RabbitMQRawMessageReceived message, CancellationToken cancellationToken)
        {
            var genericConsumeMethod = GetGenericConsumeMethod(message.MessageConfiguration.MessageType);

            object[] methodArgs = [message, cancellationToken];

            var genericConsumeResult = genericConsumeMethod.Invoke(this, methodArgs) as Task;

            await genericConsumeResult!;
        }

        private async Task ConsumeGenericAsync<TMessage>(RabbitMQRawMessageReceived message, CancellationToken cancellationToken)
        {
            var body = _serializer.Deserialize<RabbitMQMessageBody<TMessage>>(message.RawBody.Span);

            var settlement = new RabbitMQMessageSettlement<TMessage>(
                message,
                _transport,
                _logger,
                message.EndpointDefinition.MaxRedeliveryAttempts
            );

            var messageReceived = new RabbitMQMessageReceived<TMessage>(
                message.MessageId!,
                body.Message,
                message.DeliveryCount,
                message.ConsumerDefinition,
                message.EndpointDefinition,
                settlement
            );

            await _consumerExecutor.ExecuteAsync(messageReceived, cancellationToken);
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
