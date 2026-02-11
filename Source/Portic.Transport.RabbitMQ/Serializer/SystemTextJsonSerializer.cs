using System.Text.Json;

namespace Portic.Transport.RabbitMQ.Serializer
{
    internal sealed class SystemTextJsonSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };

        public T Deserialize<T>(ReadOnlySpan<byte> json)
        {
            return JsonSerializer.Deserialize<T>(json, _serializerOptions)
                ?? throw new Exception("Failed to deserialize value");
        }

        public ReadOnlyMemory<byte> Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
        }
    }
}