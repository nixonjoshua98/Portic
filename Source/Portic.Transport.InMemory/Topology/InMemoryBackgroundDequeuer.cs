using Microsoft.Extensions.Hosting;
using Portic.Transport.InMemory.Consumers;

namespace Portic.Transport.InMemory.Topology
{
    internal sealed class InMemoryBackgroundDequeuer(
        IInMemoryTransport _transport,
        InMemoryConsumerExecutor _consumerExecutor
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _transport.WaitForMessageAsync(stoppingToken);

                await _consumerExecutor.ExecuteAsync(message, stoppingToken);
            }
        }
    }
}
