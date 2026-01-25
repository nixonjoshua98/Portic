using System.Collections.Concurrent;

namespace Portic.Models
{
    internal sealed class CustomPropertyBag : ICustomPropertyBag
    {
        private readonly ConcurrentDictionary<string, object> _properties = new();

        public void SetProperty(string key, object value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            
            _properties[key] = value;
        }

        public T GetOrDefault<T>(string key, T defaultValue)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            
            return _properties.TryGetValue(key, out var value) && value is T typedValue ?
                typedValue :
                defaultValue;
        }
    }
}