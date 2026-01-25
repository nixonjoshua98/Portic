namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportPayload<TMessage> : ITransportPayload<TMessage>
    {
        public TMessage Message { get; set; }
        public DateTimeOffset PublishedAt { get; set; }

        public TransportPayload(TMessage message, DateTimeOffset publishedAt)
        {
            Message = message;
            PublishedAt = publishedAt;
        }
    }
}