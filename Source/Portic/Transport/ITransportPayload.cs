namespace Portic.Transport
{
    public interface ITransportPayload<TMessage>
    {
        TMessage Message { get; }
        string MessageId { get; }
    }
}
