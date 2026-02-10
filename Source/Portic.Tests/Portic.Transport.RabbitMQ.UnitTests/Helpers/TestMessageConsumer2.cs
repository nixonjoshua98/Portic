using Portic.Consumers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport.RabbitMQ.UnitTests.Helpers
{
    internal sealed class TestMessageConsumer2 : IConsumer<TestMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<TestMessage> context)
        {
            throw new NotImplementedException();
        }
    }
}
