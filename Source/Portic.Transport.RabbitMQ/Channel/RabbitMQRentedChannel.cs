using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Channel
{
    internal sealed class RabbitMQRentedChannel(RabbitMQChannelPool Pool, IChannel channel) : IRabbitMQRentedChannel
    {
        public IChannel Channel => channel;

        public void Dispose()
        {
            Pool.Release(channel);
        }
    }
}