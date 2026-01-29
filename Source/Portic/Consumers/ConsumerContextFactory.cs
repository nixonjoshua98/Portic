using Portic.Transport;

namespace Portic.Consumers
{
    internal sealed class ConsumerContextFactory : IConsumerContextFactory
    {
        public ValueTask<IConsumerContext<TMessage>> CreateAsync<TMessage>(
            ITransportMessageReceived<TMessage> message,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var context = new ConsumerContext<TMessage>(
                message.MessageId,
                message.Message,
                message.DeliveryCount,
                serviceProvider,
                message.ConsumerDefinition,
                message.EndpointDefinition,
                message.Settlement,
                cancellationToken
            );

            return ValueTask.FromResult<IConsumerContext<TMessage>>(context);
        }
    }
}