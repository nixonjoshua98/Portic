using Microsoft.Extensions.Hosting;
using Portic.Abstractions;
using Portic.Transport.RabbitMQ.Abstractions;
using Portic.Transport.RabbitMQ.Consumer;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Topology
{
    internal sealed class RabbitMQTopologyHostedService(
        IPorticConfiguration _dispatchConfiguration,
        IRabbitMQTopologyFactory _topologyFactory,
        IRabbitMQConnectionContext _connectionContext,
        IRabbitMQMessageConsumer _messageConsumer
    ) : IHostedLifecycleService
    {
        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            await _topologyFactory.CreateTopologyAsync(_dispatchConfiguration.Consumers, cancellationToken);

            var queues = _dispatchConfiguration.Consumers
                .Select(x => x.GetQueueName())
                .Distinct();

            foreach (var queueName in queues)
            {
                var channel = await _connectionContext.CreateChannelAsync(cancellationToken);

                var asyncConsumer = new AsyncEventingBasicConsumer(channel);

                await channel.BasicConsumeAsync(
                    queueName,
                    autoAck: true,
                    consumerTag: string.Empty,
                    noLocal: false,
                    exclusive: true,
                    arguments: null,
                    consumer: asyncConsumer,
                    cancellationToken: cancellationToken
                );

                asyncConsumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    await _messageConsumer.ConsumeAsync(eventArgs, eventArgs.CancellationToken);
                };
            }
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}