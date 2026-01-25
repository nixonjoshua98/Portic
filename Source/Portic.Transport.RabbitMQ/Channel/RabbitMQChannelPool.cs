using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Portic.Transport.RabbitMQ.Channel
{
    internal sealed class RabbitMQChannelPool(IConnection connection, int maxPoolSize = 100)
    {
        private readonly IConnection _connection = connection;
        private readonly ConcurrentQueue<RabbitMQRentableChannel> _idleChannels = new();
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
                var newChannel = await CreateChannelAsync(cancellationToken);

                return new RabbitMQRentedChannel(this, newChannel);
            }
            finally
            {
                _creationLock.Release();
            }
        }

        public void Release(RabbitMQRentableChannel channel)
        {
            _idleChannels.Enqueue(channel);
        }

        private async Task<RabbitMQRentableChannel> CreateChannelAsync(CancellationToken cancellationToken)
        {
            var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return new RabbitMQRentableChannel(channel);
        }
    }

    internal sealed class RabbitMQRentableChannel(IChannel channel)
    {
        public readonly IChannel Channel = channel;
        
        public bool IsInUse { get; private set; } = false;

        public void Rent()
        {
            IsInUse = true;
        }

        public void Release()
        {
            IsInUse = false;
        }
    }

    internal interface IRentedChannel : IDisposable
    {
        IChannel Channel { get; }
    }

    internal sealed class RabbitMQRentedChannel(
        RabbitMQChannelPool Pool, 
        RabbitMQRentableChannel rentable
    ) : IRentedChannel
    {
        public IChannel Channel => rentable.Channel;

        public void Dispose()
        {
            Pool.Release(rentable);
        }
    }
}
