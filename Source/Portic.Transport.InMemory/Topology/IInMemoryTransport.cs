namespace Portic.Transport.InMemory.Topology
{
    internal interface IInMemoryTransport : IMessageTransport
    {
        IAsyncEnumerable<InMemoryQueuedMessage> GetMessagesAsync(CancellationToken cancellationToken);
        Task PublishDeferredAsync(InMemoryQueuedMessage message, Exception exception, CancellationToken cancellationToken);
    }
}
