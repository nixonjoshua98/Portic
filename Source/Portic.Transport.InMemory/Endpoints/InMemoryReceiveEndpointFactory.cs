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
        public Task<IReceiveEndpoint> CreateReceiveEndpointAsync(IEndpointDefinition _, CancellationToken cancellationToken)
        {
            var receiver = new InMemoryReceiveEndpoint(_transport, _consumerExecutor);

            return Task.FromResult<IReceiveEndpoint>(receiver);
        }
    }
}
