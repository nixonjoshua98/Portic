using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Transport
{
    public sealed record TransportMessageReceived<TMessage>(
        string MessageId,
        TMessage Message,
        byte DeliveryCount,
        IConsumerDefinition ConsumerDefinition,
        IEndpointDefinition EndpointDefinition,
        IMessageSettlement Settlement
    );
}