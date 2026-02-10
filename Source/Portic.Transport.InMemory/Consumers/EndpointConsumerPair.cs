using Portic.Consumers;
using Portic.Endpoints;

namespace Portic.Transport.InMemory.Consumers
{
    internal readonly record struct EndpointConsumerPair(IEndpointDefinition Endpoint, IConsumerDefinition Consumer);
}
