using System.Collections.Concurrent;

namespace Portic.Models
{
    internal sealed class CustomPropertyBag : ICustomPropertyBag
    {
        private readonly ConcurrentDictionary<string, object> _properties = new();

        public void SetProperty(string key, object value)
        {
            _properties[key] = value;
        }

        public T GetOrDefault<T>(string key, T defaultValue)
        {
            return _properties.TryGetValue(key, out var value) && value is T typedValue ?
                typedValue :
                defaultValue;
        }
    }
}