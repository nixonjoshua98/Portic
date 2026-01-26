namespace Portic.Consumer
{
    internal interface IConsumerContextFactory
    {
        IConsumerContext<TMessage> CreateContext<TMessage>(ConsumerExecutorContext<TMessage> context, CancellationToken cancellationToken);
    }
}
