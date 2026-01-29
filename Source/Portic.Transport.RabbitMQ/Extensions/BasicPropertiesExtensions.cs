using RabbitMQ.Client;
using System.Text;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class BasicPropertiesExtensions
    {
        private const string MessageNameKey = "x-portic-message";
        private const string MessageIdKey = "x-portic-message-id";
        private const string DeliveryCountKey = "x-portic-delivery-count";

        extension(BasicProperties properties)
        {
            public BasicProperties SetMessageName(string name) =>
                SetHeaderValue(properties, MessageNameKey, name);

            public BasicProperties SetDeliveryCount(byte count) =>
                SetHeaderValue(properties, DeliveryCountKey, count);

            public BasicProperties SetMessageId(string id) =>
                SetHeaderValue(properties, MessageIdKey, id);

            public BasicProperties SetHeadersFrom(IReadOnlyBasicProperties source)
            {
                if (!source.IsHeadersPresent() || source.Headers is null)
                {
                    return properties;
                }

                foreach (var header in source.Headers)
                {
                    SetHeaderValue(properties, header.Key, header.Value);
                }

                return properties;
            }
        }

        extension(IReadOnlyBasicProperties properties)
        {
            public string? MessageName =>
                GetHeaderValue(properties, MessageNameKey);

            public string? PorticMessageId =>
                GetHeaderValue(properties, MessageIdKey);

            public byte DeliveryCount =>
                GetHeaderValueOrDefault<byte>(properties, DeliveryCountKey, 0);
        }

        private static BasicProperties SetHeaderValue(BasicProperties properties, string name, object? value)
        {
            properties.Headers ??= new Dictionary<string, object?>();
            properties.Headers[name] = value;
            return properties;
        }

        private static T GetHeaderValueOrDefault<T>(IReadOnlyBasicProperties properties, string name, T defaultValue) where T : IParsable<T>
        {
            if (!properties.IsHeadersPresent() || properties.Headers is null)
            {
                return defaultValue;
            }

            else if (!properties.Headers.TryGetValue(name, out var headerValue))
            {
                return defaultValue;
            }

            else if (headerValue is T parsed)
            {
                return parsed;
            }

            else if (headerValue is byte[] headerBytes)
            {
                var str = Encoding.UTF8.GetString(headerBytes);

                return T.TryParse(str, null, out var value) ?
                    value : defaultValue;
            }

            return defaultValue;
        }

        private static string? GetHeaderValue(IReadOnlyBasicProperties properties, string name)
        {
            if (!properties.IsHeadersPresent() || properties.Headers is null)
            {
                return null;
            }

            else if (properties.Headers.TryGetValue(name, out var headerValue) && headerValue is byte[] headerBytes)
            {
                return Encoding.UTF8.GetString(headerBytes);
            }

            return null;
        }
    }
}
