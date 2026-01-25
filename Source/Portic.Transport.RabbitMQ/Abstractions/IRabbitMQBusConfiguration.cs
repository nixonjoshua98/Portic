using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQBusConfiguration
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}