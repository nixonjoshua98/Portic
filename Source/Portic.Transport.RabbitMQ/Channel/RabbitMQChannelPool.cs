using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Portic.Transport.RabbitMQ.Channel
{
    internal sealed class RabbitMQChannelPool(IConnection connection, int maxPoolSize = 256)
    {
        private readonly IConnection _connection = connection;
        private readonly ConcurrentQueue<IChannel> _idleChannels = new();
        private readonly SemaphoreSlim _creationLock = new(maxPoolSize, maxPoolSize);

        public async Task<IRentedChannel> RentAsync(CancellationToken cancellationToken)
        {
            if (_idleChannels.TryDequeue(out var channel))
            {
                return new RabbitMQRentedChannel(this, channel);
            }

            await _creationLock.WaitAsync(cancellationToken);

            try
            {
                var newChannel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                return new RabbitMQRentedChannel(this, newChannel);
            }
            finally
            {
                _creationLock.Release();
            }
        }

        public void Release(IChannel channel)
        {
            _idleChannels.Enqueue(channel);
        }
    }

    internal interface IRentedChannel : IDisposable
    {
        IChannel Channel { get; }
    }

    internal sealed class RabbitMQRentedChannel(
        RabbitMQChannelPool Pool,
        IChannel channel
    ) : IRentedChannel
    {
        public IChannel Channel => channel;

        public void Dispose()
        {
            Pool.Release(channel);
        }
    }
}