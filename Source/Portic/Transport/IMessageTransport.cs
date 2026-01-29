namespace Portic.Transport
{
    public interface IMessageTransport
    {
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}