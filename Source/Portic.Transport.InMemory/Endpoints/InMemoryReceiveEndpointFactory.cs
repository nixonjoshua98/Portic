using Portic.Consumers;
using Portic.Endpoints;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Topology;

namespace Portic.Transport.InMemory.Endpoints
{
    internal sealed class InMemoryReceiveEndpointFactory(
        InMemoryTransport _transport,
        InMemoryConsumerExecutor _consumerExecutor
    ) : IReceiveEndpointFactory
    {
        public async Task<IReceiveEndpoint> CreateEndpointReceiverAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            var receiver = new InMemoryReceiveEndpoint(_transport, _consumerExecutor);

            return receiver;
        }
    }
}
