using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Exceptions;
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

        void ITransportDefinition.ValidateEndpoint(IEndpointDefinition endpointDefinition)
        {
            // Ensure that only one consumer is registered per message type for this endpoint, 
            // as we shouldn't have multiple consumers consuming the same message type from the same queue in RabbitMQ (the user should use multiple queues)

            HashSet<Type> seenMessageTypes = [];

            foreach (var consumer in endpointDefinition.ConsumerDefinitions)
            {
                if (!seenMessageTypes.Add(consumer.Message.MessageType))
                {
                    throw new RabbitMQMultipleMessageConsumerException(endpointDefinition.Name, consumer.Message.MessageType);
                }
            }
        }

        public IRabbitMQTransportDefinition Build()
        {
            return this;
        }
    }
}