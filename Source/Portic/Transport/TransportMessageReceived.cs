using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Transport
{
    public interface ITransportMessageReceived<TMessage>
    {
        IConsumerDefinition ConsumerDefinition { get; }
        byte DeliveryCount { get; }
        IEndpointDefinition EndpointDefinition { get; }
        TMessage Message { get; }
        string MessageId { get; }
        IMessageSettlement Settlement { get; }
    }
}