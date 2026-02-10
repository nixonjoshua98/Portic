using Portic.Consumers;
using Portic.Tests.Common.Helpers;

namespace Portic.Transport.RabbitMQ.IntegrationTests.Consumers
{
    internal sealed class TrackableConsumer(TrackableMessageSource<TestMessage> _taskCompletionSource) : IConsumer<TestMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<TestMessage> context)
        {
            _taskCompletionSource.SetResult(context.Message);

            return ValueTask.CompletedTask;
        }
    }
}
