using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Portic.Transport.RabbitMQ.Extensions
{
    internal static class BasicPropertiesExtensions
    {
        const string MessageNameKey = "d-messagename";

        public static BasicProperties SetMessageName(this BasicProperties properties, string name)
        {
            properties.Headers ??= new Dictionary<string, object?>();

            properties.Headers[MessageNameKey] = name;

            return properties;
        }

        public static string? GetMessageName(this BasicDeliverEventArgs args) => GetMessageName(args.BasicProperties);

        private static string? GetMessageName(IReadOnlyBasicProperties properties)
        {
            if (!properties.IsHeadersPresent() || properties.Headers is null)
            {
                return null;
            }
            else if (properties.Headers.TryGetValue(MessageNameKey, out var headerValue) && headerValue is byte[] headerBytes)
            {
                return Encoding.UTF8.GetString(headerBytes);
            }

            return null;
        }
    }
}
