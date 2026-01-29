using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Models;
using RabbitMQ.Client;

namespace Portic.Transport.RabbitMQ.Consumer
{
    internal sealed class RabbitMQEndpointState(
        IEndpointDefinition endpoint,
        Func<RawTransportMessageReceived, CancellationToken, Task> consumeFunc
    ) : IDisposable
    {
        private readonly IEndpointDefinition Endpoint = endpoint;
        private readonly List<RabbitMQEndpointConsumerState> ConsumerStates = [];
        private readonly Func<RawTransportMessageReceived, CancellationToken, Task> ConsumeFunc = consumeFunc;

        public IEnumerable<RabbitMQEndpointConsumerState> Consumers => ConsumerStates;

        public RabbitMQEndpointConsumerState AddConsumer(IChannel channel)
        {
            var consumer = new RabbitMQEndpointConsumerState(
                channel,
                Endpoint,
                ConsumeFunc
            );

            ConsumerStates.Add(consumer);

            return consumer;
        }

        public void Dispose()
        {
            lock (ConsumerStates)
            {
                foreach (var state in ConsumerStates)
                {
                    state?.Dispose();
                }

                ConsumerStates.Clear();
            }
        }
    }
}