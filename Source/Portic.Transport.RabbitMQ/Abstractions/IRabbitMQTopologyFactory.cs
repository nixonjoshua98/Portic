using Portic.Consumer;
using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Consumer;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQTopologyFactory
    {
        Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointConfiguration endpoint, CancellationToken cancellationToken);
    }
}