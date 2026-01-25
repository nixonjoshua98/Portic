namespace Portic.Consumer
{
    public interface IConsumerContext<TMessage>
    {
        TMessage Message { get; }
        TimeSpan Latency { get; }
        CancellationToken CancellationToken { get; }
    }
}
