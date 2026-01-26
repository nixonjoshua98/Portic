
namespace Portic.Consumer
{
    public interface IConsumerExecutor
    {
        Task ExecuteAsync<TMessage>(ConsumerExecutorContext<TMessage> context, CancellationToken cancellationToken);
    }
}