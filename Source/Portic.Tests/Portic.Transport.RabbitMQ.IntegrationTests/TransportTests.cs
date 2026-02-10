using Microsoft.Extensions.DependencyInjection;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.IntegrationTests.Fixtures;
using Portic.Transport.RabbitMQ.IntegrationTests.Helpers;
using Testcontainers.RabbitMq;
using Xunit;

namespace Portic.Transport.RabbitMQ.IntegrationTests
{
    public  class TransportTests(RabbitMQFixture fixture) : IClassFixture<RabbitMQFixture>
    {
        private readonly RabbitMqContainer _rabbitMqContainer = fixture.Container;

        [Fact]
        public async Task PublishAsync_ShouldPublishMessageToRabbitMQ()
        {
            // Arrange
            using var host = RabbitMQHost.CreateHost(_rabbitMqContainer);

            await host.StartAsync(CancellationToken.None);

            var completionSource = host.Services.GetRequiredService<TaskCompletionSource<TestMessage>>();

            var transport = host.Services.GetRequiredService<IMessageTransport>();

            var testMessage = new TestMessage(Guid.NewGuid());

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            // Act
            await transport.PublishAsync(testMessage, cts.Token);

            var message = await completionSource.Task.WaitAsync(cts.Token);

            // Assert
            Assert.Equal(testMessage.Value, message.Value);

            // Cleanup
            await host.StopAsync(CancellationToken.None);
        }
    }
}