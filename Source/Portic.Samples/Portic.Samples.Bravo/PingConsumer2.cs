using Microsoft.Extensions.Logging;
using Portic.Consumers;

namespace Portic.Samples.Bravo
{
    internal sealed class PingConsumer2(ILogger<PingConsumer2> _logger) : IConsumer<PingMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            _logger.LogInformation("Pong!!");

            return ValueTask.CompletedTask;
        }
    }
}
