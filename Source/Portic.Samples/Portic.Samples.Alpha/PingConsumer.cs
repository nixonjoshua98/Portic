using Microsoft.Extensions.Logging;
using Portic.Consumer;

namespace Portic.Samples.Alpha
{
    internal sealed class PingConsumer(ILogger<PingConsumer> _logger) : IMessageConsumer<PingMessage>
    {
        public async ValueTask ConsumeAsync(IMessageConsumerContext<PingMessage> context)
        {
            _logger.LogInformation("Ping : Consumed : {Latency:F2}ms", context.Latency.TotalMilliseconds);
        }
    }
}
