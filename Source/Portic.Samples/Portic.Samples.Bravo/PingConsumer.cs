using Portic.Consumers;

namespace Portic.Samples.Bravo
{
    internal sealed class PingConsumer : IConsumer<PingMessage>
    {
        private static readonly HashSet<string> _messageIdsToFail = [];

        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            if (Random.Shared.NextSingle() > 0.75f)
            {
                _messageIdsToFail.Add(context.MessageId);

                throw new Exception();
            }

            return ValueTask.CompletedTask;
        }
    }
}
