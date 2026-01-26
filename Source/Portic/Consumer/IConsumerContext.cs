namespace Portic.Consumer
{
    public interface IConsumerContext
    {
        object Message { get; }

        CancellationToken CancellationToken { get; }
    }

    public interface IConsumerContext<TMessage> : IConsumerContext
    {
        new TMessage Message { get; }
    }
}
