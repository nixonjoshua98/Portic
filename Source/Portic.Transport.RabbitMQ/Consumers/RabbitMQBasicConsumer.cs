using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Channels;
using Portic.Transport.RabbitMQ.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Consumers
{
    internal sealed class RabbitMQBasicConsumer(
        RabbitMQChannel channel,
        IEndpointDefinition endpoint,
        RabbitMQConsumerExecutor consumerExecutor
    ) : ReceiveEndpointBase,
        IAsyncBasicConsumer
    {
        private RabbitMQChannel? _channel = channel;

        public readonly IEndpointDefinition Endpoint = endpoint;

        private readonly RabbitMQConsumerExecutor ConsumerExecutor = consumerExecutor;

        public RabbitMQChannel Channel
        {
            get
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                return _channel ?? throw new InvalidOperationException($"Assigned channel for endpoint consumer '{Endpoint.Name}' is null.");
            }
        }

        IChannel? IAsyncBasicConsumer.Channel => Channel.RawChannel;

        protected override async Task StartAsync(CancellationToken cancellationToken)
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

            await CompletedTask.WaitAsync(cancellationToken);
        }

        public Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
        {
            SetCompleted();

            return Task.CompletedTask;
        }

        public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
        {
            SetCompleted();

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
            SetCompleted();

            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

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
    }
}