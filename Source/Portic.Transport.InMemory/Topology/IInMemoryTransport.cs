namespace Portic.Transport.InMemory.Topology
{
    internal interface IInMemoryTransport : IMessageTransport
    {
        Task<InMemoryQueuedMessage> WaitForMessageAsync(CancellationToken cancellationToken);
        Task PublishDeferredAsync(InMemoryQueuedMessage message, Exception exception, CancellationToken cancellationToken);
    }
}
