using Portic.Endpoints;
using Portic.Messages;
using Portic.Middleware;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyDictionary<Type, IMessageDefinition> messages,
        IEnumerable<IEndpointDefinition> endpoints,
        IReadOnlyList<IMiddlewareDefinition> globalMiddlewareTypes
    ) : IPorticConfiguration
    {
        private readonly IReadOnlyDictionary<Type, IMessageDefinition> Messages = messages;

        public IReadOnlyList<IEndpointDefinition> Endpoints { get; } = [.. endpoints.Where(e => e.ConsumerDefinitions.Count > 0)];

        public IReadOnlyList<IMiddlewareDefinition> Middleware { get; } = globalMiddlewareTypes;

        public IMessageDefinition GetMessageDefinition<TMessage>() => Messages[typeof(TMessage)];
    }
}