using Portic.Transport.RabbitMQ.Abstractions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQConnectionContext(
        IRabbitMQBusConfiguration _configuration
    ) : IRabbitMQConnectionContext
    {
        private readonly SemaphoreSlim ConnectionLock = new(1, 1);

        private IConnection? Connection;

        public async ValueTask<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync(cancellationToken);

            return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public async ValueTask<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await ConnectionLock.WaitAsync(cancellationToken);

                Connection ??= await _configuration.CreateConnectionAsync(cancellationToken);

                return Connection ?? throw new InvalidOperationException("Failed to create RabbitMQ connection.");
            }
            finally
            {
                ConnectionLock.Release();
            }
        }
    }
}