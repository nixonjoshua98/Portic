namespace Portic.Transport.RabbitMQ.Channels
{
    internal readonly record struct RabbitMQChannelOptions(
        uint PrefetchSize,
        ushort PrefetchCount
    );
}
