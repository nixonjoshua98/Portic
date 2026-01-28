namespace Portic.Transport.RabbitMQ.Models
{
    internal sealed record RabbitMQMessageBody<TMessage>(
        string MessageId,
        TMessage Message
    );
}