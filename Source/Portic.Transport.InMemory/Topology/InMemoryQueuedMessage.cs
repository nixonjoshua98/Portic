using Portic.Consumers;
using Portic.Endpoints;
using Portic.Messages;

namespace Portic.Transport.InMemory.Topology
{
    internal sealed record InMemoryQueuedMessage(
        string MessageId,
        object Message,
        byte DeliveryCount,
        IMessageDefinition MessageDefinition,
        IConsumerDefinition ConsumerDefinition,
        IEndpointDefinition EndpointDefinition
    );
}
