using Portic.Transport;

namespace Portic.Consumers
{
    public interface IConsumerExecutor
    {
        Task ExecuteAsync<TMessage>(TransportMessageReceived<TMessage> message, CancellationToken cancellationToken);
    }
}