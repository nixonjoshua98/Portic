using Portic.Transport.RabbitMQ.Channel;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQConnectionContext
    {
        ValueTask<IChannel> CreateChannelAsync(RabbitMQChannelOptions options, CancellationToken cancellationToken = default);
        ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
        ValueTask<IRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default);
    }
}
