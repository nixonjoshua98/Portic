using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQConnectionContext
    {
        ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
        ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    }
}
