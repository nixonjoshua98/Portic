using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Portic.Transport.RabbitMQ.Channel
{
    internal sealed class RabbitMQChannelPool(IConnection connection, int maxPoolSize = 256) : IDisposable
    {
        private readonly IConnection _connection = connection;
        private readonly ConcurrentQueue<IChannel> _idleChannels = new();
        private readonly SemaphoreSlim _creationLock = new(maxPoolSize, maxPoolSize);

        private bool _isDisposed;

        public async Task<IRabbitMQRentedChannel> RentAsync(CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

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

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _creationLock.Dispose();

                    while (_idleChannels.TryDequeue(out var channel))
                    {
                        channel?.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}