using Microsoft.Extensions.Logging;
using Portic.Consumer;

namespace Portic.Samples.Bravo
{
    internal sealed class PingConsumer(ILogger<PingConsumer> _logger) : IConsumer<PingMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            _logger.LogInformation("Ping!");

            return ValueTask.CompletedTask;
        }
    }
}
