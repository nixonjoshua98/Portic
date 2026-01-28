namespace Portic.Transport
{
    public sealed record ReceivedTransportMessage(
        string MessageId,
        string EndpointName,
        string MessageName
    );
}
