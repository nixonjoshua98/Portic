using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Channel;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQConnectionContext(
        IRabbitMQTransportConfiguration _configuration
    ) : IRabbitMQConnectionContext
    {
        private readonly SemaphoreSlim ConnectionLock = new(1, 1);

        private IConnection? Connection;

        private RabbitMQChannelPool ChannelPool = null!;

        public async ValueTask<IRentedChannel> RentChannelAsync(CancellationToken cancellationToken = default)
        {
            _ = await GetConnectionAsync(cancellationToken); // Force

            return await ChannelPool.RentAsync(cancellationToken);
        }

        public async ValueTask<IChannel> CreateChannelAsync(RabbitMQChannelOptions options, CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync(cancellationToken);

            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.BasicQosAsync(options.PrefetchSize, options.PrefetchCount, global: false, cancellationToken);

            return channel;
        }

        public async ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await ConnectionLock.WaitAsync(cancellationToken);

                Connection ??= await _configuration.CreateConnectionAsync(cancellationToken);

                ChannelPool ??= new RabbitMQChannelPool(Connection);

                return Connection ?? throw new InvalidOperationException("Failed to create RabbitMQ connection.");
            }
            finally
            {
                ConnectionLock.Release();
            }
        }
    }
}