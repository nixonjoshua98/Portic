using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Transport
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

        public IRabbitMQTransportConfigurator WithConnectionString(string connectionString)
        {
            ConnectionFactory.Uri = new Uri(connectionString);
            return this;
        }

        public IRabbitMQTransportConfigurator WithUserName(string username)
        {
            ConnectionFactory.UserName = username;
            return this;
        }

        public IRabbitMQTransportConfigurator WithPassword(string password)
        {
            ConnectionFactory.Password = password;
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