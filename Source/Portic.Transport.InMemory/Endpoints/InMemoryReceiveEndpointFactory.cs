using Portic.Endpoints;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Transport;

namespace Portic.Transport.InMemory.Endpoints
{
    internal sealed class InMemoryReceiveEndpointFactory(
        InMemoryTransport _transport,
        InMemoryConsumerExecutor _consumerExecutor
    ) : IReceiveEndpointFactory
    {
        public async Task<IReceiveEndpoint> CreateReceiveEndpointAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            var receiver = new InMemoryReceiveEndpoint(_transport, _consumerExecutor);

            return receiver;
        }
    }
}
