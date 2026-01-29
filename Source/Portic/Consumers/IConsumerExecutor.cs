using Portic.Transport;

namespace Portic.Consumers
{
    public interface IConsumerExecutor
    {
        Task ExecuteAsync<TMessage>(ITransportMessageReceived<TMessage> message, CancellationToken cancellationToken);
    }
}