using Portic.Endpoint;

namespace Portic.Consumer
{
    public sealed record TransportMessageReceived<TMessage>(
        string MessageId,
        TMessage Message,
        byte DeliveryCount,
        IConsumerConfiguration ConsumerConfiguration,
        IEndpointConfiguration EndpointConfiguration
    );
}
