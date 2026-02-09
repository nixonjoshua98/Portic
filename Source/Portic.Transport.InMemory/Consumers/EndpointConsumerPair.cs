using Portic.Consumers;
using Portic.Endpoints;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.InMemory.Consumers
{
    internal readonly record struct EndpointConsumerPair(IEndpointDefinition Endpoint, IConsumerDefinition Consumer);
}
