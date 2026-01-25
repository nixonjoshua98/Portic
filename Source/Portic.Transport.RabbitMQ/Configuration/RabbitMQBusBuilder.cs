using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Configuration
{
    internal sealed class RabbitMQBusBuilder : IRabbitMQBusBuilder, IRabbitMQBusConfiguration
    {
        private readonly IPorticConfigurator Builder;

        private readonly ConnectionFactory ConnectionFactory = new();

        public RabbitMQBusBuilder(IPorticConfigurator builder)
        {
            Builder = builder;
        }

        public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return await ConnectionFactory.CreateConnectionAsync(cancellationToken);
        }

        public IRabbitMQBusConfiguration Build()
        {
            return this;
        }
    }
}