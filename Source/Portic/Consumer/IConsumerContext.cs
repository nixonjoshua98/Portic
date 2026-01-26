namespace Portic.Consumer
{
    public interface IConsumerContext
    {
        string MessageName { get; }
        CancellationToken CancellationToken { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
        IServiceProvider Services { get; }
        IConsumerConfiguration Consumer { get; }
    }
}
