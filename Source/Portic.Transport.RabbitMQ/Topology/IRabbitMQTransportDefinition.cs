using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal interface IRabbitMQTransportDefinition : ITransportDefinition
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}