using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portic.Transport;

namespace Portic.Samples.Bravo
{
    internal sealed class PingPublisher(
        IMessageTransport _messageBus,
        ILogger<PingPublisher> _logger
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new PingMessage(
                    Random.Shared.Next()
                );

                await _messageBus.PublishAsync(message, stoppingToken);

                _logger.LogInformation("Ping");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}