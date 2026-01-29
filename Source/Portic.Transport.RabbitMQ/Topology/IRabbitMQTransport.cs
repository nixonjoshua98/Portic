using Portic.Transport.RabbitMQ.Messages;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task PublishFaultedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken);
    }
}
