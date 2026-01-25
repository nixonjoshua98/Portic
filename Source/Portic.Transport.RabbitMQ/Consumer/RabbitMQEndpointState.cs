using Portic.Endpoint;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQEndpointState(
        IEndpointConfiguration endpoint,
        Func<TransportMessageReceived, CancellationToken, Task> consumeFunc
    )
    {
        private readonly IEndpointConfiguration Endpoint = endpoint;
        private readonly List<RabbitMQEndpointConsumerState> ConsumerStates = [];
        private readonly Func<TransportMessageReceived, CancellationToken, Task> ConsumeFunc = consumeFunc;

        public RabbitMQEndpointConsumerState AddConsumer(IChannel channel, CancellationToken cancellationToken)
        {
            var consumer = new RabbitMQEndpointConsumerState(
                channel,
                Endpoint,
                ConsumeFunc,
                cancellationToken
            );

            ConsumerStates.Add(consumer);

            return consumer;
        }
    }
}