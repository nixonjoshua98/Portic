using Portic.Transport.RabbitMQ.Channels;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQConnectionContext
    {
        ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
        ValueTask<IRabbitMQRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default);
    }
}