using Portic.Consumers;

namespace Portic.Tests.Common.Helpers
{
    public sealed class TestMessageConsumer : IConsumer<TestMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<TestMessage> context)
        {
            throw new NotImplementedException();
        }
    }
}
