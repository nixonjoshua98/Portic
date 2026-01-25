using Microsoft.Extensions.Hosting;
using Portic.Transport;

namespace Portic.Samples.Alpha
{
    internal sealed class PingPublisher(
        IMessageTransport _messageBus
    ) : BackgroundService
    {
        readonly TimeSpan PingInterval = TimeSpan.FromMilliseconds(1 / 100);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pingMessage = new PingMessage();

                await _messageBus.PublishAsync(pingMessage, stoppingToken);

                await Task.Delay(PingInterval, stoppingToken);
            }
        }
    }
}