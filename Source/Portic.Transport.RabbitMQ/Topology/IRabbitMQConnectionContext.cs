using Portic.Transport.RabbitMQ.Channel;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQConnectionContext
    {
        ValueTask<IChannel> CreateChannelAsync(RabbitMQChannelOptions options, CancellationToken cancellationToken = default);
        ValueTask<IRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default);
    }
}
