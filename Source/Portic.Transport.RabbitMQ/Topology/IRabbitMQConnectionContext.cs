using Portic.Transport.RabbitMQ.Channels;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQConnectionContext
    {
        ValueTask<IChannel> CreateChannelAsync(RabbitMQChannelOptions options, CancellationToken cancellationToken = default);
        ValueTask<IRabbitMQRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default);
    }
}