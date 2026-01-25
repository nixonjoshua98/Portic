namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed class TransportMessagePayload<TMessage> : ITransportPayload<TMessage>
    {
        public TMessage Message { get; set; }
        public DateTimeOffset PublishedAt { get; set; }

        public TransportMessagePayload(TMessage message, DateTimeOffset publishedAt)
        {
            Message = message;
            PublishedAt = publishedAt;
        }
    }
}