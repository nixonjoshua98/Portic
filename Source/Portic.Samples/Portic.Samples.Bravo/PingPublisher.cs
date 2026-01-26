using Microsoft.Extensions.Hosting;
using Portic.Transport;

namespace Portic.Samples.Bravo
{
    internal sealed class PingPublisher(
        IMessageTransport _messageBus
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _messageBus.PublishAsync(new PingMessage(), stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}