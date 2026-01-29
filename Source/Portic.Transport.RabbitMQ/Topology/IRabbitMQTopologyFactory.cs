using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Consumers;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTopologyFactory
    {
        Task<RabbitMQEndpointState> CreateEndpointStateAsync(IEndpointDefinition endpoint, CancellationToken cancellationToken);
    }
}