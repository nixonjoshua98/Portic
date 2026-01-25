using Portic.Abstractions;
using Portic.Endpoint;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyDictionary<Type, IMessageConfiguration> messages,
        IReadOnlyList<IEndpointConfiguration> endpoints
    ) : IPorticConfiguration
    {
        private readonly IReadOnlyDictionary<Type, IMessageConfiguration> Messages = messages;

        public IReadOnlyList<IEndpointConfiguration> Endpoints =>
            [.. endpoints.Where(e => e.Consumers.Any())];

        public IMessageConfiguration GetMessageConfiguration<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages[messageType];
        }
    }
}