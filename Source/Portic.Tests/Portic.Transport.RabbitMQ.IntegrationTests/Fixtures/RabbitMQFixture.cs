using Testcontainers.RabbitMq;
using Xunit;

namespace Portic.Transport.RabbitMQ.IntegrationTests.Fixtures
{
    public sealed class RabbitMQFixture : IAsyncLifetime
    {
        public RabbitMqContainer Container { get; }

        public RabbitMQFixture()
        {
            Container = new RabbitMqBuilder("rabbitmq:3-management-alpine")
                .WithPortBinding(5672, true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }
    }
}