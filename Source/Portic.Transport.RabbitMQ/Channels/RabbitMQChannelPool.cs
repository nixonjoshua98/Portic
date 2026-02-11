using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Portic.Transport.RabbitMQ.Channels
{
    internal sealed class RabbitMQChannelPool(IConnection connection, ILoggerFactory loggerFactory) : IDisposable
    {
        private bool _isDisposed;

        private readonly IConnection _connection = connection;
        private readonly ConcurrentQueue<IChannel> _idleChannels = new();
        private readonly ILogger<RabbitMQChannel> _rabbitMqChannelLogger = loggerFactory.CreateLogger<RabbitMQChannel>();

        public async Task<RabbitMQChannel> GetNonRentedChannelAsync(CancellationToken cancellationToken)
        {
            var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return WrapChannel(channel);
        }

        public async Task<RabbitMQChannel> GetChannelAsync(CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_idleChannels.TryDequeue(out var channel))
            {
                return WrapChannel(channel);
            }

            channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return WrapChannel(channel);
        }

        private RabbitMQChannel WrapChannel(IChannel rawChannel)
        {
            return new RabbitMQChannel(rawChannel, _rabbitMqChannelLogger, this);
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