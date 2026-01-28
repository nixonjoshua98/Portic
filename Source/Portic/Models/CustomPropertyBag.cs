namespace Portic.Models
{
    internal sealed class CustomPropertyBag : IReadOnlyCustomPropertyBag
    {
        private readonly Dictionary<string, object> _properties = [];

        public void Set(string key, object value)
        {
            _properties[key] = value;
        }

        public T GetOrDefault<T>(string key, T defaultValue)
        {
            return _properties.TryGetValue(key, out var value) && value is T typedValue ?
                typedValue : defaultValue;
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }
    }
}