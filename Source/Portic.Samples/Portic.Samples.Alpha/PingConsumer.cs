using Portic.Consumer;

namespace Portic.Samples.Alpha
{
    internal sealed class PingConsumer : IConsumer<PingMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            return ValueTask.CompletedTask;
        }
    }
}
