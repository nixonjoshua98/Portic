using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Configuration
{
    internal sealed class RabbitMQTransport : IRabbitMQTransportConfigurator, IRabbitMQTransportConfiguration
    {
        private readonly IPorticConfigurator Builder;

        private readonly ConnectionFactory ConnectionFactory = new();

        public RabbitMQTransport(IPorticConfigurator builder)
        {
            Builder = builder;
        }

        public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return await ConnectionFactory.CreateConnectionAsync(cancellationToken);
        }

        public IRabbitMQTransportConfiguration Build()
        {
            return this;
        }
    }
}