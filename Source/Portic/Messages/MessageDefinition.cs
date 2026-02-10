using Portic.Models;

namespace Portic.Messages
{
    internal sealed class MessageDefinition(
        string name,
        Type messageType,
        IReadOnlyCustomPropertyBag properties
    ) : IMessageDefinition
    {
        private readonly IReadOnlyCustomPropertyBag _properties = properties;

        public Type MessageType { get; } = messageType;
        public string Name { get; } = name;

        public T GetPropertyOrDefault<T>(string key, T defaultValue) =>
            _properties.GetOrDefault(key, defaultValue);
    }
}