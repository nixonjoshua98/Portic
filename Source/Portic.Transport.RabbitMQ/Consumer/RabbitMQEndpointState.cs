using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQEndpointState
    {
        public readonly IChannel Channel;
        public readonly IEndpointConfiguration Endpoint;

        private readonly AsyncEventingBasicConsumer EventConsumer;
        private readonly CancellationToken CancellationToken;
        private readonly Func<TransportMessageReceived, CancellationToken, Task> ConsumeFunc;

        public RabbitMQEndpointState(
            IChannel channel,
            IEndpointConfiguration endpoint,
            Func<TransportMessageReceived, CancellationToken, Task> consumeFunc,
            CancellationToken cancellationToken)
        {
            Channel = channel;
            Endpoint = endpoint;
            ConsumeFunc = consumeFunc;
            CancellationToken = cancellationToken;

            EventConsumer = new AsyncEventingBasicConsumer(channel);

            EventConsumer.ReceivedAsync += EventConsumer_ReceivedAsync;
        }

        public async Task BasicConsumeAsync()
        {
            await Channel.BasicConsumeAsync(
                Endpoint.Name,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: true,
                arguments: null,
                consumer: EventConsumer,
                cancellationToken: CancellationToken
            );
        }

        private async Task EventConsumer_ReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            await ConsumeFunc(
                new TransportMessageReceived(
                    this,
                    args
                ),
                args.CancellationToken
            );
        }
    }
}
