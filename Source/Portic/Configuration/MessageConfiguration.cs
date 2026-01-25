using Portic.Abstractions;

namespace Portic.Configuration
{
    internal sealed class MessageConfiguration(string name, Type messageType) : IMessageConfiguration
    {
        public Type MessageType { get; } = messageType;
        public string Name { get; } = name;
    }
}
