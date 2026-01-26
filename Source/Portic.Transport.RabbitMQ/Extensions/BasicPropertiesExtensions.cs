using RabbitMQ.Client;
using System.Text;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class BasicPropertiesExtensions
    {
        private const string MessageNameKey = "x-portic-message";
        private const string DeliveryCountKey = "x-portic-delivery-count";

        public static BasicProperties SetMessageName(this BasicProperties properties, string name)
        {
            properties.Headers ??= new Dictionary<string, object?>();

            properties.Headers[MessageNameKey] = name;

            return properties;
        }

        public static string? GetMessageName(this IReadOnlyBasicProperties properties) =>
            GetHeaderValue(properties, MessageNameKey);

        public static byte GetDeliveryCount(this IReadOnlyBasicProperties properties) =>
            GetHeaderValueOrDefault<byte>(properties, DeliveryCountKey, 0);

        private static T GetHeaderValueOrDefault<T>(IReadOnlyBasicProperties properties, string name, T defaultValue) where T : IParsable<T>
        {
            var str = GetHeaderValue(properties, name);

            if (string.IsNullOrEmpty(str) || !T.TryParse(str, null, out var value))
            {
                return defaultValue;
            }

            return value;
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
