using Portic.Models;

namespace Portic.Messages
{
    internal sealed class MessageConfigurator(Type messageType) : IMessageConfigurator
    {
        private readonly CustomPropertyBag _properties = new();

        public Type MessageType { get; } = messageType;
        public string Name { get; private set; } = messageType.FullName ?? messageType.Name;

        public IMessageConfigurator WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            Name = name;
            return this;
        }

        public IMessageConfigurator SetProperty(string key, object value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            _properties.Set(key, value);
            return this;
        }

        public IMessageDefinition ToDefinition()
        {
            return new MessageDefinition(Name, MessageType, _properties);
        }
    }
}
