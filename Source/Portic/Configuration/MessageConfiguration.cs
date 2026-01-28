using Portic.Abstractions;
using Portic.Models;

namespace Portic.Configuration
{
    internal sealed class MessageConfiguration(
        string name,
        Type messageType,
        IReadOnlyCustomPropertyBag properties
    ) : IMessageConfiguration
    {
        private readonly IReadOnlyCustomPropertyBag _properties = properties;

        public Type MessageType { get; } = messageType;
        public string Name { get; } = name;

        public T GetPropertyOrDefault<T>(string key, T defaultValue) =>
            _properties.GetOrDefault(key, defaultValue);
    }
}
