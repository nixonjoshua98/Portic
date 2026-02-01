using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTransportDefinition : IRabbitMQTransportConfigurator, IRabbitMQTransportDefinition
    {
        private readonly ConnectionFactory ConnectionFactory = new();

        public string DisplayName => "RabbitMQ";

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

        public IRabbitMQTransportDefinition Build()
        {
            return this;
        }
    }
}