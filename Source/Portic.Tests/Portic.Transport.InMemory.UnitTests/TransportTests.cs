using Portic.Configuration;
using Portic.Messages;
using Portic.Tests.Common.Helpers;
using Portic.Tests.Common.Mocks;
using Portic.Transport.InMemory.Topology;
using Xunit;

namespace Portic.Transport.InMemory.UnitTests
{
    public class TransportTests
    {
        [Fact]
        public async Task PublishDeferredAsync_ShouldIncrementDeliveryCount()
        {
            var messageDefinition = new MockMessageDefinition(typeof(TestMessage), "TestMessage");
            var consumerDefinition = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer", messageDefinition);
            var endpointDefinition = new MockEndpointDefinition("TestEndpoint", 0, [consumerDefinition]);

            var mockConfiguration = new PorticConfiguration(
                new Dictionary<Type, IMessageDefinition>() 
                { 
                    [messageDefinition.MessageType] = messageDefinition
                },
                [endpointDefinition],
                []
            );

            var transport = new InMemoryTransport(mockConfiguration);

            var originalMessage = new InMemoryQueuedMessage(
                string.Empty,
                new TestMessage(),
                0,
                messageDefinition,
                consumerDefinition,
                endpointDefinition
            );

            await transport.PublishDeferredAsync(originalMessage, new Exception(), CancellationToken.None);

            await foreach (var queuedMessage in transport.GetMessagesAsync(CancellationToken.None))
            {
                Assert.Equal(1, queuedMessage.DeliveryCount);

                return; // We only need the single message published
            }
        }
    }
}
