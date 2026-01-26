
namespace Portic.Consumer
{
    public interface IConsumerExecutor
    {
        Task ExecuteAsync<TMessage>(IConsumerContext<TMessage> executorContext, CancellationToken cancellationToken);
    }
}