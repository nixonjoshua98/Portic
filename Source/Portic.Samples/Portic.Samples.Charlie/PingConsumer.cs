using Microsoft.Extensions.Logging;
using Portic.Consumers;

namespace Portic.Samples.Alpha
{
    internal sealed class PingConsumer(ILogger<PingConsumer> _logger) : IConsumer<PingMessage>
    {
        public ValueTask ConsumeAsync(IConsumerContext<PingMessage> context)
        {
            _logger.LogInformation("Pong!");

            return ValueTask.CompletedTask;
        }
    }
}
