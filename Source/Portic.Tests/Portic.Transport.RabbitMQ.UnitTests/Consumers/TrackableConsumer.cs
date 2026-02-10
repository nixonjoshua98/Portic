using Portic.Consumers;
using Portic.Tests.Common.Helpers;

namespace Portic.Transport.RabbitMQ.UnitTests.Consumers
{
    internal sealed class TrackableConsumer(TaskCompletionSource<TestMessage> _taskCompletionSource) : IConsumer<TestMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<TestMessage> context)
        {
            _taskCompletionSource.SetResult(context.Message);

            return ValueTask.CompletedTask;
        }
    }
}
