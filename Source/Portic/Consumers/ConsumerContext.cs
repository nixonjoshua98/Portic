using Portic.Endpoints;
using Portic.Messages;

namespace Portic.Consumers
{
    internal sealed class ConsumerContext<TMessage>(
        string messageId,
        TMessage message,
        byte deliveryCount,
        IServiceProvider serviceProvider,
        IConsumerDefinition consumer,
        IEndpointDefinition endpoint,
        CancellationToken cancellationToken
    ) : IConsumerContext<TMessage>
    {
        public string MessageId { get; } = messageId;
        public string MessageName { get; } = consumer.Message.Name;
        public TMessage Message { get; } = message;
        public IMessageDefinition MessageConfiguration { get; } = consumer.Message;
        public CancellationToken CancellationToken { get; } = cancellationToken;
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
