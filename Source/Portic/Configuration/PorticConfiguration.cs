using Portic.Abstractions;
using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyList<IMessageConfiguration> messages,
        IReadOnlyList<IEndpointConfiguration> endpoints
    ) : IPorticConfiguration
    {
        public IReadOnlyList<IMessageConfiguration> Messages { get; } = messages;

        public IReadOnlyList<IEndpointConfiguration> Endpoints =>
            [.. endpoints.Where(e => e.Consumers.Any())];

        public IMessageConfiguration GetMessageConfiguration<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages.FirstOrDefault(m => m.MessageType == messageType)
                ?? throw new InvalidOperationException($"No message configuration found for message type: {messageType.FullName}");
        }
    }
}