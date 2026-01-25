using Portic.Consumer;

namespace Portic.Samples.Alpha
{
    internal sealed class PingConsumer : IMessageConsumer<PingMessage>
    {
        public ValueTask ConsumeAsync(IMessageConsumerContext<PingMessage> context)
        {
            return ValueTask.CompletedTask;
        }
    }
}
