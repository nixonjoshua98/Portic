namespace Portic.Consumer
{
    public interface IConsumerContext
    {
        string MessageId { get; }
        string MessageName { get; }
        IServiceProvider Services { get; }
        CancellationToken CancellationToken { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
        IConsumerConfiguration Consumer { get; }
    }
}
