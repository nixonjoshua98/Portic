using Portic.Endpoints;
using Portic.Messages;
using Portic.Transport;
using System.Diagnostics.CodeAnalysis;

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

        public bool TryGetEndpointByName(string name, [NotNullWhen(true)] out IEndpointDefinition? endpoint)
        {
            endpoint = Endpoints.FirstOrDefault(e => e.Name == name);
            return endpoint is not null;
        }

        public IMessageDefinition GetMessageConfiguration<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages[messageType];
        }
    }
}