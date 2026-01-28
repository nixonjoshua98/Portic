using Microsoft.Extensions.Logging;
using Portic.Consumer;

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

                throw new Exception($"ConsumerConfiguration for message '{context.MessageId}' has failed");
            }

            return ValueTask.CompletedTask;
        }
    }
}
