using Portic.Endpoints;
using Portic.Transport;

namespace Portic.Consumers
{
    internal sealed class ConsumerContext<TMessage>(
        string messageId,
        TMessage message,
        byte deliveryCount,
        IServiceProvider serviceProvider,
        IConsumerDefinition consumer,
        IEndpointDefinition endpoint,
        IMessageSettlement messageSettlement,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public string MessageId { get; } = messageId;
        public TMessage Message { get; } = message;
        public CancellationToken CancellationToken { get; } = cancellationToken;
        public IMessageSettlement Settlement { get; } = messageSettlement;
        public IServiceProvider Services { get; private set; } = serviceProvider;
        public IConsumerDefinition ConsumerDefinition { get; } = consumer;
        public byte DeliveryCount { get; } = deliveryCount;
        public byte MaxRedeliveryAttempts { get; } = endpoint.MaxRedeliveryAttempts;

        public IConsumerContext WithServiceProvider(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

            Services = serviceProvider;

            return this;
        }
    }
}