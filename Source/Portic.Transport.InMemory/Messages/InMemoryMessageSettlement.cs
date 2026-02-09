using Portic.Transport.InMemory.Topology;

namespace Portic.Transport.InMemory.Messages
{
    internal sealed class InMemoryMessageSettlement(InMemoryQueuedMessage Message, IInMemoryTransport Transport) : IMessageSettlement
    {
        public Task CompleteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task DeferAsync(Exception exception, CancellationToken cancellationToken)
        {
            await Transport.PublishDeferredAsync(Message, exception, cancellationToken);
        }

        public Task FaultAsync(Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
