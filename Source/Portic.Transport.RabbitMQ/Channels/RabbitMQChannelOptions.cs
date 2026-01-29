namespace Portic.Transport.RabbitMQ.Channel
{
    internal readonly record struct RabbitMQChannelOptions(
        uint PrefetchSize,
        ushort PrefetchCount
    );
}
