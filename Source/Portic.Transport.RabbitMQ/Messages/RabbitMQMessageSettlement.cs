using Microsoft.Extensions.Logging;
using Portic.Transport.RabbitMQ.Logging;
using Portic.Transport.RabbitMQ.Topology;

namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed class RabbitMQMessageSettlement<TMessage>(
        RabbitMQRawMessageReceived RawMessage,
        IRabbitMQTransport Transport,
        ILogger _logger,
        byte MaxRedeliveryAttempts
    ) : IMessageSettlement
    {
        public async Task CompleteAsync(CancellationToken cancellationToken)
        {
            await RawMessage.Channel.BasicAckAsync(RawMessage.DeliveryTag, false, cancellationToken);
        }

        public async Task DeferAsync(CancellationToken cancellationToken)
        {
            // Republish the message for redelivery first, to ensure at-least-once delivery guarantee
            await Transport.PublishFaultedAsync(RawMessage, cancellationToken);

            // Ack the original message to remove it from the queue
            // Intentionally ignoring cancellationToken here to ensure Ack is sent regardless of cancellation
            await RawMessage.Channel.BasicAckAsync(RawMessage.DeliveryTag, false, CancellationToken.None);

            _logger.LogSuccessfulRedelivery(RawMessage.MessageId, RawMessage.DeliveryCount + 1, MaxRedeliveryAttempts);
        }

        public async Task FaultAsync(Exception exception, CancellationToken cancellationToken)
        {
            // Eventually this will be a DLQ or similar handling

            await RawMessage.Channel.BasicNackAsync(RawMessage.DeliveryTag, false, false, cancellationToken);
        }
    }
}
