using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Consumer;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTopologyFactory
    {
        Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointConfiguration endpoint, CancellationToken cancellationToken);
    }
}