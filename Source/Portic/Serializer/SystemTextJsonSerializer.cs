using System.Text.Json;

namespace Portic.Serializer
{
    internal sealed class SystemTextJsonSerializer : IPorticSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public T Deserialize<T>(ReadOnlySpan<byte> json)
        {
            return JsonSerializer.Deserialize<T>(json, _serializerOptions)
                ?? throw new Exception("Failed to deserialize value");
        }

        public byte[] SerializeToBytes<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);
        }
    }
}