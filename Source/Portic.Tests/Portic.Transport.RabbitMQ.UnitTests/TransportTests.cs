using Microsoft.Extensions.DependencyInjection;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.UnitTests.Testers;
using Testcontainers.RabbitMq;
using Xunit;

namespace Portic.Transport.RabbitMQ.UnitTests
{
    public class TransportTests : IAsyncLifetime
    {
        private readonly RabbitMqContainer _rabbitMqContainer;

        public TransportTests()
        {
            _rabbitMqContainer = new RabbitMqBuilder("rabbitmq:3-management-alpine")
                .WithPortBinding(5672, true)
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.DisposeAsync();
        }

        [Fact]
        public async Task PublishAsync_ShouldPublishMessageToRabbitMQ()
        {
            using var host = RabbitMQHost.CreateHost(_rabbitMqContainer);

            await host.StartAsync(CancellationToken.None);

            await using var serviceScope = host.Services.CreateAsyncScope();

            var transport = serviceScope.ServiceProvider.GetRequiredService<IMessageTransport>();

            var testMessage = new TestMessage();

            await transport.PublishAsync(testMessage, CancellationToken.None);

            Assert.True(true);
        }
    }
}