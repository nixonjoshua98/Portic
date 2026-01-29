using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Transport.RabbitMQ.Messages
{
    internal sealed record RabbitMQMessageReceived<TMessage>(
        string MessageId,
        TMessage Message,
        byte DeliveryCount,
        IConsumerDefinition ConsumerDefinition,
        IEndpointDefinition EndpointDefinition,
        IMessageSettlement Settlement
    ) : ITransportMessageReceived<TMessage>;
}
