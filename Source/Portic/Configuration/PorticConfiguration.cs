using Portic.Abstractions;
using Portic.Endpoint;
using System.Diagnostics.CodeAnalysis;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyDictionary<Type, IMessageConfiguration> messages,
        IReadOnlyList<IEndpointConfiguration> endpoints,
        IReadOnlyList<Type> globalMiddlewareTypes
    ) : IPorticConfiguration
    {
        private readonly IReadOnlyDictionary<Type, IMessageConfiguration> Messages = messages;

        public IReadOnlyList<IEndpointConfiguration> Endpoints { get; } = [.. endpoints.Where(e => e.Consumers.Any())];

        public IReadOnlyList<Type> Middleware { get; } = globalMiddlewareTypes;

        public bool TryGetEndpointByName(string name, [NotNullWhen(true)] out IEndpointConfiguration? endpoint)
        {
            endpoint = Endpoints.FirstOrDefault(e => e.Name == name);
            return endpoint is not null;
        }

        public IMessageConfiguration GetMessageConfiguration<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages[messageType];
        }
    }
}