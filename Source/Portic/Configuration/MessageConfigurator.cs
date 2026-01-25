using Portic.Abstractions;

namespace Portic.Configuration
{
    internal sealed class MessageConfigurator : IMessageConfigurator
    {
        public Type MessageType { get; }

        public MessageConfigurator(Type messageType)
        {
            MessageType = messageType;
        }

        public IMessageConfiguration Build()
        {
            return new MessageConfiguration(MessageType);
        }
    }
}
