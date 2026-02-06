using Microsoft.Extensions.Logging;
using Portic.Transport.RabbitMQ.Logging;
using Portic.Transport.RabbitMQ.Topology;

namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed class RabbitMQMessageSettlement<TMessage>(
        RabbitMQRawMessageReceived RawMessage,
        IRabbitMQTransport Transport
    ) : IMessageSettlement
    {
        public async Task CompleteAsync(CancellationToken cancellationToken)
        {
            await RawMessage.Channel.BasicAckAsync(RawMessage.DeliveryTag, false, cancellationToken);
        }

        public async Task DeferAsync(Exception exception, CancellationToken cancellationToken)
        {
            // Republish the message for redelivery first, to ensure at-least-once delivery guarantee
            await Transport.PublishDeferedAsync(RawMessage, exception, cancellationToken);

            // Ack the original message to remove it from the queue
            // Intentionally ignoring cancellationToken here to ensure Ack is sent regardless of cancellation
            await RawMessage.Channel.BasicAckAsync(RawMessage.DeliveryTag, false, CancellationToken.None);
        }

        public async Task FaultAsync(Exception exception, CancellationToken cancellationToken)
        {
            await Transport.PublishFaultedAsync(RawMessage, exception, cancellationToken);

            // Ack the original message to remove it from the queue
            // Intentionally ignoring cancellationToken here to ensure Ack is sent regardless of cancellation
            await RawMessage.Channel.BasicAckAsync(RawMessage.DeliveryTag, false, CancellationToken.None);
        }
    }
}
