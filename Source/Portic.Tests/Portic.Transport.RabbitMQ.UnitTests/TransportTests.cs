using Microsoft.Extensions.DependencyInjection;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.UnitTests.Helpers;
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
            await _rabbitMqContainer.StopAsync();
        }

        [Fact]
        public async Task PublishAsync_ShouldPublishMessageToRabbitMQ()
        {
            // Arrange

            using var host = RabbitMQHost.CreateHost(_rabbitMqContainer);

            await host.StartAsync(CancellationToken.None);

            await using var serviceScope = host.Services.CreateAsyncScope();

            var completionSource = serviceScope.ServiceProvider.GetRequiredService<TaskCompletionSource<TestMessage>>();

            var transport = serviceScope.ServiceProvider.GetRequiredService<IMessageTransport>();

            var testMessage = new TestMessage();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            // Act

            await transport.PublishAsync(testMessage, cts.Token);

            var message = await completionSource.Task.WaitAsync(cts.Token);

            // Assert

            Assert.Equal(testMessage, message);
        }
    }
}