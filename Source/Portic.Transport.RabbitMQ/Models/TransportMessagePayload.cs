namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportMessagePayload<TMessage>(
        string messageId,
        TMessage message
    ) : ITransportPayload<TMessage>
    {
        public string MessageId { get; set; } = messageId;
        public TMessage Message { get; set; } = message;
    }
}