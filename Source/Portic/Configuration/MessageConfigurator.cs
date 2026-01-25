using Portic.Abstractions;

namespace Portic.Configuration
{
    internal sealed class MessageConfigurator(Type messageType) : IMessageConfigurator
    {
        public Type MessageType { get; } = messageType;
        public string Name { get; private set; } = messageType.FullName ?? messageType.Name;

        public IMessageConfigurator WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            Name = name;

            return this;
        }

        public IMessageConfiguration Build()
        {
            return new MessageConfiguration(Name, MessageType);
        }
    }
}
