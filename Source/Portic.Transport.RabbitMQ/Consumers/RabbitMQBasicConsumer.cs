using Portic.Consumers;
using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Consumers
{
    internal sealed class RabbitMQBasicConsumer :
        IReceiveEndpoint,
        IAsyncBasicConsumer
    {
        private bool _isDisposed;
        private IChannel? _channel;

        public readonly IEndpointDefinition Endpoint;

        private readonly RabbitMQConsumerExecutor ConsumerExecutor;

        public IChannel Channel
        {
            get
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                return _channel ?? throw new InvalidOperationException($"Assigned channel for endpoint consumer '{Endpoint.Name}' is null.");
            }
        }

        public RabbitMQBasicConsumer(
            IChannel channel,
            IEndpointDefinition endpoint,
            RabbitMQConsumerExecutor consumerExecutor)
        {
            _channel = channel;
            Endpoint = endpoint;
            ConsumerExecutor = consumerExecutor;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await BasicConsumeAsync(cancellationToken);
        }

        public Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task HandleBasicDeliverAsync(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IReadOnlyBasicProperties properties,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken = default)
        {
            await ConsumerExecutor.ExecuteAsync(
                new RabbitMQRawMessageReceived(
                    this,
                    body,
                    deliveryTag,
                    properties
                ),
                cancellationToken
            );
        }

        public Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
        {
            return Task.CompletedTask;
        }

        private async Task BasicConsumeAsync(CancellationToken cancellationToken)
        {
            await Channel.BasicConsumeAsync(
                Endpoint.Name,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: this,
                cancellationToken: cancellationToken
            );
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _channel?.Dispose();
                }

                _channel = null;
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