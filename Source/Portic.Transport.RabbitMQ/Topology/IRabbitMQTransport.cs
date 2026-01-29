using Portic.Transport.RabbitMQ.Messages;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task PublishFaultedAsync(RabbitMQRawMessageReceived message, CancellationToken cancellationToken);
    }
}
