using Portic.Exceptions;
using Portic.Tests.Common.Helpers;
using Portic.Tests.Common.Mocks;
using Portic.Validation;
using Xunit;

namespace Portic.Transport.RabbitMQ.UnitTests
{
    public class DefinitionTests
    {
        [Fact]
        public void ValidateEndpoint_ShouldThrow_WhenMultipleMessageConsumersRegistered()
        {
            // Arrange
            var messageDefinition = new MockMessageDefinition(typeof(TestMessage), "TestMessage");

            var consumer1 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer", messageDefinition);
            var consumer2 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer2", messageDefinition);

            var endpoint = new MockEndpointDefinition("TestMessage", 0, [consumer1, consumer2]);

            // Assert
            Assert.Throws<MultipleMessageConsumerException>(() => CommonDefinitionValidator.ValidateSingleMessageConsumerEndpoint(endpoint));
        }
    }
}