namespace Portic.Consumer
{
    public interface IConsumerContext
    {
        string MessageId { get; }
        string MessageName { get; }
        byte DeliveryCount { get; }
        IServiceProvider Services { get; }
        CancellationToken CancellationToken { get; }

        internal IConsumerConfiguration Consumer { get; }

        IConsumerContext WithServiceProvider(IServiceProvider serviceProvider);
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        TMessage Message { get; }
    }
}
