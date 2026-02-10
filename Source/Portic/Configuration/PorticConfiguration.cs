using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyDictionary<Type, IMessageDefinition> messages,
        IEnumerable<IEndpointDefinition> endpoints,
        IReadOnlyList<Type> globalMiddlewareTypes,
        ITransportDefinition transportDefinition
    ) : IPorticConfiguration
    {
        private readonly IReadOnlyDictionary<Type, IMessageDefinition> Messages = messages;

        public IReadOnlyList<IEndpointDefinition> Endpoints { get; } = [.. endpoints.Where(e => e.ConsumerDefinitions.Count > 0)];

        public ITransportDefinition TransportDefinition { get; } = transportDefinition;

        public IReadOnlyList<Type> Middleware { get; } = globalMiddlewareTypes;

        public IMessageDefinition GetMessageDefinition<TMessage>() => Messages[typeof(TMessage)];
    }
}