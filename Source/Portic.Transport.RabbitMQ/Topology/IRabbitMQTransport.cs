using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Messages;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransport : IMessageTransport
    {
        Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointDefinition endpoint, CancellationToken cancellationToken);
        Task PublishFaultedAsync(RabbitMQRawMessageReceived message, Exception exception, CancellationToken cancellationToken);
    }
}
