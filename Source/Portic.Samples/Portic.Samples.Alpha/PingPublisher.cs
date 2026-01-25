using Portic.Samples.Alpha;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portic.Transport;

namespace Portic.Samples.Alpha
{
    internal sealed class PingPublisher(
        IMessageBus _messageBus,
        ILogger<PingPublisher> _logger
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pingMessage = new PingMessage();

                await _messageBus.PublishAsync(pingMessage, stoppingToken);

                _logger.LogInformation("Ping : Published");
            }
        }
    }
}
