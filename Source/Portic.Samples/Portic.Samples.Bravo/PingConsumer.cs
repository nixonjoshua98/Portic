using Microsoft.Extensions.Logging;
using Portic.Consumer;

namespace Portic.Samples.Bravo
{
    internal sealed class PingConsumer(ILogger<PingConsumer> _logger) : IConsumer<PingMessage>
    {
        private static readonly HashSet<string> _messageIdsToFail = [];

        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            bool skip = _messageIdsToFail.Remove(context.MessageId);

            if (!skip && Random.Shared.NextSingle() > 0.5f)
            {
                _messageIdsToFail.Add(context.MessageId);

                throw new Exception($"Consumer for message '{context.MessageId}' has failed");
            }

            _logger.LogInformation("Ping!");

            return ValueTask.CompletedTask;
        }
    }
}
