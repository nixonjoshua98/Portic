using Portic.Abstractions;

namespace Portic.Configuration
{
    internal sealed class MessageConfiguration : IMessageConfiguration
    {
        public Type MessageType { get; }

        public MessageConfiguration(Type messageType)
        {
            MessageType = messageType;
        }

        public string GetName()
        {
            return MessageType.FullName!;
        }
    }
}
