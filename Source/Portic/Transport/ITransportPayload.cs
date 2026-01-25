namespace Portic.Transport
{
    public interface ITransportPayload<TMessage>
    {
        DateTimeOffset PublishedAt { get; }
        TMessage Message { get; }
    }
}
