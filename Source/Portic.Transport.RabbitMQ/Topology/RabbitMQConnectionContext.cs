using Portic.Transport.RabbitMQ.Channels;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQConnectionContext(IRabbitMQTransportDefinition _configuration) : IDisposable
    {
        private bool _isDisposed;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        private IConnection? _connection;
        private RabbitMQChannelPool? _channelPool;

        public async ValueTask<RabbitMQRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            var channelPool = await GetChannelPoolAsync(cancellationToken);

            return await channelPool.RentAsync(cancellationToken);
        }

        public async ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            var connection = await GetConnectionAsync(cancellationToken);

            return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        private async ValueTask<RabbitMQChannelPool> GetChannelPoolAsync(CancellationToken cancellationToken = default)
        {
            // Skip accessing the connection + lock
            if (_channelPool is not null)
            {
                return _channelPool;
            }

            var connection = await GetConnectionAsync(cancellationToken);

            return _channelPool ??= new RabbitMQChannelPool(connection);
        }

        private async ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            // Skip acquiring the lock
            if (_connection is not null)
            {
                return _connection;
            }

            try
            {
                await _connectionLock.WaitAsync(cancellationToken);

                _connection ??= await _configuration.CreateConnectionAsync(cancellationToken);

                return _connection ?? throw new InvalidOperationException("Failed to create RabbitMQ connection.");
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                    _channelPool?.Dispose();
                    _connectionLock?.Dispose();
                }

                _connection = null;
                _channelPool = null;

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