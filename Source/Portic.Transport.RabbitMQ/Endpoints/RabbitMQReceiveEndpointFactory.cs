using Portic.Consumers;
using Portic.Endpoints;
using Portic.Transport.RabbitMQ.Consumers;
using Portic.Transport.RabbitMQ.Extensions;
using Portic.Transport.RabbitMQ.Transport;

namespace Portic.Transport.RabbitMQ.Endpoints
{
    internal sealed class RabbitMQReceiveEndpointFactory(
        RabbitMQConnectionContext _connectionContext,
        RabbitMQConsumerExecutor _consumerExecutor
    ) : IReceiveEndpointFactory
    {
        public async Task<IReceiveEndpoint> CreateEndpointReceiverAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            var channel = await _connectionContext.GetChannelAsync(cancellationToken);

            await channel.BasicQosAsync(0, endpointDefinition.PrefetchCount, global: false, cancellationToken);

            foreach (var consumerDefinition in endpointDefinition.ConsumerDefinitions)
            {
                await channel.BindConsumerToEndpointAsync(endpointDefinition, consumerDefinition, cancellationToken);
            }

            return new RabbitMQBasicConsumer(
                channel,
                endpointDefinition,
                _consumerExecutor
            );
        }
    }
}