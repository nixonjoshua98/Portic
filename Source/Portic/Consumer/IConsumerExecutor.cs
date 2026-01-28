
namespace Portic.Consumer
{
    public interface IConsumerExecutor
    {
        Task ExecuteAsync<TMessage>(TransportMessageReceived<TMessage> message, CancellationToken cancellationToken);
    }
}