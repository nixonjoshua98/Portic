using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Configuration
{
    internal sealed class RabbitMQTransportConfiguration : IRabbitMQTransportConfigurator, IRabbitMQTransportConfiguration
    {
        private readonly IPorticConfigurator Configurator;
        private readonly ConnectionFactory ConnectionFactory = new();

        public RabbitMQTransportConfiguration(IPorticConfigurator builder)
        {
            Configurator = builder;
        }

        public IRabbitMQTransportConfigurator WithHost(string hostName)
        {
            ConnectionFactory.HostName = hostName;
            return this;
        }

        public IRabbitMQTransportConfigurator WithPort(int port)
        {
            ConnectionFactory.Port = port;
            return this;
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