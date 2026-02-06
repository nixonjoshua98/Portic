using Portic.Transport.RabbitMQ.Messages;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task PublishDeferredAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken);
        Task PublishFaultedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken);
    }
}
