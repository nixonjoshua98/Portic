using Portic.Abstractions;

namespace Portic.Configuration
{
    internal sealed class MessageConfigurator(Type messageType) : IMessageConfigurator
    {
        public Type MessageType { get; } = messageType;
        public string Name { get; private set; } = messageType.FullName ?? messageType.Name;

        public IMessageConfiguration Build()
        {
            return new MessageConfiguration(Name, MessageType);
        }
    }
}
