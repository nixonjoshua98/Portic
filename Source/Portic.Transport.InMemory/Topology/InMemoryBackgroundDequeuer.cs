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
            await foreach (var message in _transport.GetMessagesAsync(stoppingToken))
            {
                await _consumerExecutor.ExecuteAsync(message, stoppingToken);
            }
        }
    }
}