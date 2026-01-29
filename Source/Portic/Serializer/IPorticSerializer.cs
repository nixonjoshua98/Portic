namespace Portic.Serializer
{
    public interface IPorticSerializer
    {
        T Deserialize<T>(ReadOnlySpan<byte> json);
        ReadOnlyMemory<byte> Serialize<T>(T value);
    }
}
