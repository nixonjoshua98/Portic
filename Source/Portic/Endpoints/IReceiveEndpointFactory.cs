using Portic.Consumers;

namespace Portic.Endpoints
{
    public interface IReceiveEndpointFactory
    {
        Task<IReceiveEndpoint> CreateEndpointReceiverAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken);
    }
}
