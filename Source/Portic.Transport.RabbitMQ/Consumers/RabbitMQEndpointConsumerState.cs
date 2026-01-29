using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQEndpointConsumerState : IDisposable
    {
        private bool _isDisposed;

        public IChannel? Channel { get; private set; }

        public readonly IEndpointDefinition Endpoint;

        private readonly AsyncEventingBasicConsumer EventConsumer;
        private readonly Func<RawTransportMessageReceived, CancellationToken, Task> ConsumeFunc;

        public RabbitMQEndpointConsumerState(
            IChannel channel,
            IEndpointDefinition endpoint,
            Func<RawTransportMessageReceived, CancellationToken, Task> consumeFunc)
        {
            Channel = channel;
            Endpoint = endpoint;
            ConsumeFunc = consumeFunc;

            EventConsumer = new AsyncEventingBasicConsumer(channel);

            EventConsumer.ReceivedAsync += EventConsumer_ReceivedAsync;
        }

        public async Task BasicConsumeAsync(CancellationToken cancellationToken)
        {
            var channel = GetChannelOrThrow();

            await channel.BasicConsumeAsync(
                Endpoint.Name,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: EventConsumer,
                cancellationToken: cancellationToken
            );
        }

        private async Task EventConsumer_ReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            await ConsumeFunc(
                new RawTransportMessageReceived(
                    this,
                    args
                ),
                args.CancellationToken
            );
        }

        public IChannel GetChannelOrThrow()
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            return Channel ?? throw new InvalidOperationException($"Assigned channel for endpoint consumer '{Endpoint.Name}' is null.");
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Channel?.Dispose();
                }

                Channel = null;
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}