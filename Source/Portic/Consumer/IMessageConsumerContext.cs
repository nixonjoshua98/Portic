namespace Portic.Consumer
{
    public interface IMessageConsumerContext<TMessage>
    {
        TMessage Message { get; }
        TimeSpan Latency { get; }
        CancellationToken CancellationToken { get; }
    }
}
