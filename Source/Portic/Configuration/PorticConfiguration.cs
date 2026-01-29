using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyDictionary<Type, IMessageDefinition> messages,
        IReadOnlyList<IEndpointDefinition> endpoints,
        IReadOnlyList<Type> globalMiddlewareTypes,
        ITransportDefinition transportDefinition
    ) : IPorticConfiguration
    {
        private readonly IReadOnlyDictionary<Type, IMessageDefinition> Messages = messages;

        public IReadOnlyList<IEndpointDefinition> Endpoints { get; } = [.. endpoints.Where(e => e.Consumers.Any())];

        public ITransportDefinition TransportDefinition { get; } = transportDefinition;

        public IReadOnlyList<Type> Middleware { get; } = globalMiddlewareTypes;

        public IMessageDefinition GetMessageDefinition<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages[messageType];
        }
    }
}