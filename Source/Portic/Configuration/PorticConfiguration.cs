using Portic.Abstractions;
using Portic.Consumer;
using Portic.Endpoint;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration(
        IReadOnlyList<IMessageConsumerConfiguration> consumers,
        IReadOnlyList<IMessageConfiguration> messages,
        IReadOnlyList<IEndpointConfiguration> endpoints
    ) : IPorticConfiguration
    {
        public IReadOnlyList<IMessageConsumerConfiguration> Consumers { get; } = consumers;

        public IReadOnlyList<IMessageConfiguration> Messages { get; } = messages;

        public IReadOnlyList<IEndpointConfiguration> Endpoints =>
            [.. endpoints.Where(e => e.Consumers.Any())];

        public IMessageConfiguration GetMessageConfiguration<TMessage>()
        {
            var messageType = typeof(TMessage);

            return Messages.FirstOrDefault(m => m.MessageType == messageType)
                ?? throw new InvalidOperationException($"No message configuration found for message type: {messageType.FullName}");
        }

        public IMessageConsumerConfiguration? GetConsumerForMessage(IMessageConfiguration messageConfiguration)
        {
            return Consumers
                .Where(x => x.Message.MessageType == messageConfiguration.MessageType)
                .SingleOrDefault();
        }

        public IMessageConfiguration GetMessageConfiguration(string messageName)
        {
            return Messages.FirstOrDefault(m => m.GetName() == messageName)
                ?? throw new InvalidOperationException($"No message configuration found for message: {messageName}");
        }
    }
}