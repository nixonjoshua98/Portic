namespace Portic.Serializer
{
    public interface IPorticSerializer
    {
        T Deserialize<T>(ReadOnlySpan<byte> json);

        byte[] SerializeToBytes<T>(T value);
    }
}
