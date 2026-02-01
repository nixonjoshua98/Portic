using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Channels
{
    internal sealed class RabbitMQRentedChannel(RabbitMQChannelPool Pool, IChannel channel) : IDisposable
    {
        public IChannel Channel => channel;

        public void Dispose()
        {
            Pool.Release(channel);
        }
    }
}