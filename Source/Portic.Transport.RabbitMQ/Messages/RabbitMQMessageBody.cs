namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed record RabbitMQMessageBody<TMessage>(
        TMessage Message
    );
}