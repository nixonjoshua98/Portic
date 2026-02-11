using Microsoft.Extensions.DependencyInjection;
using Portic.Tests.Common.Helpers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Tests.Fixtures;
using Portic.Transport.RabbitMQ.Tests.Helpers;
using Testcontainers.RabbitMq;
using Xunit;

namespace Portic.Transport.RabbitMQ.Tests
{
    public class TransportTests(RabbitMQFixture fixture) : IClassFixture<RabbitMQFixture>
    {
        private readonly RabbitMqContainer _rabbitMqContainer = fixture.Container;

        [Fact]
        public async Task PublishAsync_ShouldPublishMessageToRabbitMQ()
        {
            // Arrange
            using var host = RabbitMQHost.CreateHost(_rabbitMqContainer);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            await host.StartAsync(cts.Token);

            var tracked = host.Services.GetRequiredService<TrackableMessageSource<TestMessage>>();

            var transport = host.Services.GetRequiredService<IMessageTransport>();

            var testMessage = new TestMessage(Guid.NewGuid());

            // Act
            await transport.PublishAsync(testMessage, cts.Token);

            var message = await tracked.WaitAsync(cts.Token);

            // Assert
            Assert.Equal(testMessage.Value, message.Value);
        }
    }
}