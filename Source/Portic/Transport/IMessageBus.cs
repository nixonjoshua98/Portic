namespace Portic.Transport
{
    public interface IMessageBus
    {
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    }
}
