using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Abstractions
{
    internal interface IRabbitMQTransportConfiguration
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}