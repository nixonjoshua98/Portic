using Portic.Abstractions;
using Portic.Consumer;

namespace Portic.Configuration
{
    internal sealed class PorticConfiguration : IPorticConfiguration
    {
        public IReadOnlyList<IMessageConsumerConfiguration> Consumers { get; }
        public IReadOnlyList<IMessageConfiguration> Messages { get; }

        public PorticConfiguration(
            IReadOnlyList<IMessageConsumerConfiguration> consumers,
            IReadOnlyList<IMessageConfiguration> messages)
        {
            Consumers = consumers;
            Messages = messages;
        }

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