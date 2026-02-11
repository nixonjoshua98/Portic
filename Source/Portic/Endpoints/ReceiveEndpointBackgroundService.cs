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
        private readonly List<IReceiveEndpoint> _receiverEndpoints = [];
        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                DisposeEndpoints();
            });

            foreach (var endpointDefinition in _porticConfigurator.Endpoints)
            {
                var receiver = await _endpointFactory.CreateReceiveEndpointAsync(endpointDefinition, cancellationToken);

                foreach (var consumer in endpointDefinition.ConsumerDefinitions)
                {
                    _logger.LogEndpointConsumerCreation(endpointDefinition.Name, consumer.ConsumerType.Name ?? string.Empty);
                }

                _receiverEndpoints.Add(receiver);
            }
        }

        public async Task StartedAsync(CancellationToken cancellationToken)
        {
            _ = Task.WhenAll(_receiverEndpoints.Select(x => x.RunAsync(cancellationToken)));
        }

        private void DisposeEndpoints()
        {
            _receiverEndpoints.ForEach(x => x.Dispose());
            _receiverEndpoints.Clear();
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}