using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransportConfiguration
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}