using Portic.Endpoints;
using Portic.Transport.InMemory.Consumers;
using Portic.Transport.InMemory.Transport;

namespace Portic.Transport.InMemory.Endpoints
{
    internal sealed class InMemoryReceiveEndpoint(InMemoryTransport _transport, InMemoryConsumerExecutor _consumerExecutor) : ReceiveEndpointBase
    {
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await foreach (var message in _transport.GetMessagesAsync(cancellationToken))
            {
                await _consumerExecutor.ExecuteAsync(message, cancellationToken);
            }
        }
    }
}