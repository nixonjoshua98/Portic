using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Portic.Configuration;
using Portic.Consumers;
using Portic.Logging;

namespace Portic.Endpoints
{
    internal sealed class ReceiveEndpointBackgroundService(
        IPorticConfiguration _porticConfigurator,
        IReceiveEndpointFactory _endpointFactory,
        ILogger<ReceiveEndpointBackgroundService> _logger
    ) : IHostedLifecycleService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            List<IReceiveEndpoint> receiverEndpoints = [];

            cancellationToken.Register(() =>
            {
                receiverEndpoints.ForEach(x => x.Dispose());
                receiverEndpoints.Clear();
            });

            foreach (var endpointDefinition in _porticConfigurator.Endpoints)
            {
                var receiver = await _endpointFactory.CreateEndpointReceiverAsync(endpointDefinition, cancellationToken);

                foreach (var consumer in endpointDefinition.ConsumerDefinitions)
                {
                    _logger.LogEndpointConsumerCreation(endpointDefinition.Name, consumer.ConsumerType.Name ?? string.Empty);
                }

                receiverEndpoints.Add(receiver);

                _ = receiver.RunAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}