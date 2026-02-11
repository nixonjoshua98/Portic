using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portic.Configuration;
using Portic.Logging;

namespace Portic.Endpoints
{
    internal sealed class ReceiveEndpointBackgroundService(
        IPorticConfiguration _porticConfigurator,
        IReceiveEndpointFactory _endpointFactory,
        ILogger<ReceiveEndpointBackgroundService> _logger
    ) : IHostedLifecycleService
    {
        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            List<IReceiveEndpoint> receiverEndpoints = [];

            cancellationToken.Register(() =>
            {
                DisposeEndpoints(receiverEndpoints);
            });

            foreach (var endpointDefinition in _porticConfigurator.Endpoints)
            {
                var receiver = await _endpointFactory.CreateReceiveEndpointAsync(endpointDefinition, cancellationToken);

                foreach (var consumer in endpointDefinition.ConsumerDefinitions)
                {
                    _logger.LogEndpointConsumerCreation(endpointDefinition.Name, consumer.ConsumerType.Name ?? string.Empty);
                }

                receiverEndpoints.Add(receiver);

                _ = receiver.StartAsync(cancellationToken);
            }
        }

        private static void DisposeEndpoints(List<IReceiveEndpoint> endpoints)
        {
            endpoints.ForEach(x => x.Dispose());
            endpoints.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}