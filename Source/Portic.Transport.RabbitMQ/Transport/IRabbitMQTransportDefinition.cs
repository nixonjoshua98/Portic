using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Transport
{
    internal interface IRabbitMQTransportDefinition : ITransportDefinition
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}