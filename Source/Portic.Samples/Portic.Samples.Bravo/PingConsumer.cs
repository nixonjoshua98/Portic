using Microsoft.Extensions.Logging;
using Portic.Consumer;

namespace Portic.Samples.Bravo
{
    internal sealed class PingConsumer(ILogger<PingConsumer> _logger) : IConsumer<PingMessage>
    {
        private static readonly HashSet<string> _messageIdsToFail = [];

        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            if (Random.Shared.NextSingle() > 0.75f)
            {
                _messageIdsToFail.Add(context.MessageId);

                throw new Exception($"Consumer for message '{context.MessageId}' has failed");
            }

            _logger.LogInformation("Pong! {Value}", context.Message.Value);

            return ValueTask.CompletedTask;
        }
    }
}
