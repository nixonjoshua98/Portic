using Portic.Tests.Common.Helpers;
using Portic.Tests.Common.Mocks;
using Portic.Transport.RabbitMQ.Exceptions;
using Portic.Transport.RabbitMQ.Topology;
using Xunit;

namespace Portic.Transport.RabbitMQ.UnitTests
{
    public class DefinitionTests
    {
        [Fact]
        public void ValidateEndpoint_ShouldAllowOnlyOneMessageConsumer()
        {
            // Arrange
            var messageDefinition = new MockMessageDefinition(typeof(TestMessage), "TestMessage");

            var consumer1 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer", messageDefinition);
            var consumer2 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer2", messageDefinition);

            var endpoint = new MockEndpointDefinition("TestMessage", 0, [consumer1, consumer2]);

            ITransportDefinition transportDefinition = new RabbitMQTransportDefinition();

            // Assert
            Assert.Throws<RabbitMQMultipleMessageConsumerException>(() => transportDefinition.ValidateEndpoint(endpoint));
        }
    }
}