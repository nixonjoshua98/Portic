using Portic.Messages;
using Portic.Tests.Common.Mocks;
using Portic.Transport.RabbitMQ.Exceptions;
using Portic.Transport.RabbitMQ.Topology;
using Portic.Transport.RabbitMQ.UnitTests.Helpers;
using Xunit;

namespace Portic.Transport.RabbitMQ.UnitTests
{
    public class DefinitionTests
    {
        private readonly IMessageDefinition TestMessageDefinition = new MockMessageDefinition(typeof(TestMessage), "TestMessage");

        [Fact]
        public void AllowOnlyOneMessageConsumerPerEndpoint()
        {
            var consumer1 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer", TestMessageDefinition);
            var consumer2 = new MockConsumerDefinition(typeof(TestMessageConsumer), "TestMessageConsumer2", TestMessageDefinition);

            var endpoint = new MockEndpointDefinition("TestMessage", 0, [consumer1, consumer2]);

            ITransportDefinition transportDefinition = new RabbitMQTransportDefinition();

            Assert.Throws<RabbitMQMultipleMessageConsumerException>(() => transportDefinition.ValidateEndpoint(endpoint));
        }
    }
}